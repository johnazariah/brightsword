#!/usr/bin/env python3
"""bump_versions.py

Usage:
  bump_versions.py --project BrightSword.SwissKnife --new-version 1.0.11
  bump_versions.py --project BrightSword.Feber --increment patch

This script reads versions.json at the repository root, updates the requested project's
version (either set to --new-version or increment the patch with --increment patch),
optionally increments dependent projects (simple transitive logic based on a hardcoded
dependency map), updates csproj AssemblyVersion/FileVersion/Version, and writes back
versions.json. It prints the resulting versions as KEY=VALUE lines for CI consumption.

This is intentionally minimal and conservative; extend dependency_map as needed.
"""
import argparse
import json
import os
import re
import sys
from xml.etree import ElementTree as ET

ROOT = os.path.dirname(os.path.dirname(__file__))
VERSIONS_PATH = os.path.join(ROOT, 'versions.json')

def discover_dependency_map(root_dir):
    """Discover reverse dependency map by scanning ProjectReference entries.

    Returns a dict: project -> [dependent-projects]
    where project and dependent-projects are folder names matching the project directories.
    """
    deps = {}
    # Limit discovered projects to those present in versions.json (avoid tests and helper projects)
    try:
        with open(VERSIONS_PATH, 'r', encoding='utf-8') as vf:
            known_projects = set(json.load(vf).keys())
    except Exception:
        known_projects = None
    # Find all csproj files under root_dir
    for dirpath, dirnames, filenames in os.walk(root_dir):
        for fn in filenames:
            if fn.endswith('.csproj'):
                proj_path = os.path.join(dirpath, fn)
                try:
                    tree = ET.parse(proj_path)
                    root = tree.getroot()
                except Exception:
                    continue
                # project folder name (assumes proj lives in a folder named after project)
                proj_folder = os.path.basename(dirpath)
                if known_projects is not None and proj_folder not in known_projects:
                    # skip projects not listed in versions.json
                    continue
                # ensure key exists
                deps.setdefault(proj_folder, [])
                # find ProjectReference Include attributes
                for pr in root.findall('.//ProjectReference'):
                    include = pr.get('Include')
                    if not include:
                        continue
                    # resolve referenced project path and get its folder name
                    ref_path = os.path.normpath(os.path.join(dirpath, include))
                    ref_folder = os.path.basename(os.path.dirname(ref_path))
                    if known_projects is not None and ref_folder not in known_projects:
                        continue
                    # record that ref_folder is depended on by proj_folder
                    deps.setdefault(ref_folder, [])
                    if proj_folder not in deps[ref_folder]:
                        deps[ref_folder].append(proj_folder)
    return deps


def discover_all_projects(root_dir):
    """Return a set of project folder names that contain a .csproj file."""
    projects = set()
    for dirpath, dirnames, filenames in os.walk(root_dir):
        for fn in filenames:
            if fn.endswith('.csproj'):
                proj_folder = os.path.basename(dirpath)
                projects.add(proj_folder)
    return projects


def read_versions():
    # Read versions.json and ensure all discovered projects are present
    with open(VERSIONS_PATH, 'r', encoding='utf-8') as f:
        raw = json.load(f)
    # Exclude test projects from baseline
    base = {k: v for k, v in raw.items() if not k.endswith('.Tests')}
    # Discover projects in the repo and ensure they exist in the versions map
    discovered = discover_all_projects(ROOT)
    for proj in discovered:
        if proj not in base:
            # try to read existing <Version> from csproj if present
            csproj_path = os.path.join(ROOT, proj, f'{proj}.csproj')
            ver = None
            if os.path.exists(csproj_path):
                try:
                    tree = ET.parse(csproj_path)
                    pg = tree.getroot().find('PropertyGroup')
                    if pg is not None:
                        v_el = pg.find('Version')
                        if v_el is not None and v_el.text:
                            ver = v_el.text.strip()
                except Exception:
                    ver = None
            base[proj] = ver or '0.1.0'
    return base


def write_versions(d):
    # Ensure test projects are not written back
    out = {k: v for k, v in d.items() if not k.endswith('.Tests')}
    with open(VERSIONS_PATH, 'w', encoding='utf-8') as f:
        json.dump(out, f, indent=2)


def bump_patch(version):
    parts = version.split('.')
    if len(parts) < 3:
        parts += ['0'] * (3 - len(parts))
    parts[2] = str(int(parts[2]) + 1)
    return '.'.join(parts[:3])


def set_csproj_versions(project, semver):
    proj_dir = os.path.join(ROOT, project)
    csproj_name = f'{project}.csproj'
    csproj_path = os.path.join(proj_dir, csproj_name)
    if not os.path.exists(csproj_path):
        print(f'WARN: csproj not found for {project} at {csproj_path}', file=sys.stderr)
        return

    tree = ET.parse(csproj_path)
    root = tree.getroot()
    ns = {'msbuild': 'http://schemas.microsoft.com/developer/msbuild/2003'}

    # Find or create PropertyGroup
    pg = None
    for child in root.findall('PropertyGroup'):
        pg = child
        break
    if pg is None:
        pg = ET.SubElement(root, 'PropertyGroup')

    # Update or add Version, AssemblyVersion, FileVersion
    def set_or_update(tag, value):
        el = pg.find(tag)
        if el is None:
            el = ET.SubElement(pg, tag)
        el.text = value

    # semver is MAJOR.MINOR.PATCH
    # Log previous version for debugging
    prev = None
    v_el = pg.find('Version')
    if v_el is not None and v_el.text:
        prev = v_el.text.strip()
    print(f'Updating {project}: Version {prev or "<none>"} -> {semver}')
    set_or_update('Version', semver)
    # Assembly/File version often have four segments; append .0
    set_or_update('AssemblyVersion', semver + '.0')
    set_or_update('FileVersion', semver + '.0')

    # Write back with minimal formatting
    tree.write(csproj_path, encoding='utf-8', xml_declaration=True)


def main():
    p = argparse.ArgumentParser()
    p.add_argument('--project', required=True)
    p.add_argument('--new-version')
    p.add_argument('--increment', choices=['patch'], help='increment the version')
    args = p.parse_args()

    versions = read_versions()
    if args.new_version:
        versions[args.project] = args.new_version
    elif args.increment == 'patch':
        versions[args.project] = bump_patch(versions.get(args.project, '0.0.0'))
    else:
        print('Either --new-version or --increment patch is required', file=sys.stderr)
        sys.exit(2)

    # Discover dependency map dynamically and increment dependents' patch versions
    dependency_map = discover_dependency_map(ROOT)
    queue = [args.project]
    visited = set()
    while queue:
        cur = queue.pop(0)
        if cur in visited:
            continue
        visited.add(cur)
        for dep in dependency_map.get(cur, []):
            versions[dep] = bump_patch(versions.get(dep, '0.0.0'))
            queue.append(dep)

    # Update csproj files
    for proj, ver in versions.items():
        set_csproj_versions(proj, ver)

    write_versions(versions)

    # Also write a machine-readable bumped_versions.json for CI consumption
    bumped_path = os.path.join(ROOT, 'bumped_versions.json')
    try:
        with open(bumped_path, 'w', encoding='utf-8') as bf:
            json.dump({k: v for k, v in versions.items() if not k.endswith('.Tests')}, bf)
    except Exception as ex:
        print(f'WARN: failed to write bumped_versions.json: {ex}', file=sys.stderr)

    # If running inside GitHub Actions, export outputs to the file referenced by GITHUB_OUTPUT
    github_out = os.getenv('GITHUB_OUTPUT')
    if github_out:
        try:
            # Prepare the JSON for the bumped versions (exclude test projects)
            bumped = {k: v for k, v in versions.items() if not k.endswith('.Tests')}
            # Write multi-line JSON safely using the heredoc-style required by GITHUB_OUTPUT
            with open(github_out, 'a', encoding='utf-8') as gout:
                gout.write('bumped_versions<<EOF\n')
                json.dump(bumped, gout)
                gout.write('\nEOF\n')
                # Also set individual simple outputs for easy consumption
                sk = bumped.get('BrightSword.SwissKnife', '')
                fb = bumped.get('BrightSword.Feber', '')
                gout.write(f'SwissKnifeVersion={sk}\n')
                gout.write(f'FeberVersion={fb}\n')
        except Exception as ex:
            print(f'WARN: failed to write GITHUB_OUTPUT: {ex}', file=sys.stderr)

    # Print environment-friendly output (exclude test projects)
    for proj, ver in versions.items():
        if proj.endswith('.Tests'):
            continue
        print(f'{proj}={ver}')


if __name__ == '__main__':
    main()

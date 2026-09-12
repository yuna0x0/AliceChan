# Changelog

All notable changes to this package are recorded here. The format follows
[Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and the package follows
[Semantic Versioning](https://semver.org/).

## [Unreleased]

## [2.0.0] - 2026-09-07

### Changed

- Package renamed from `com.edisonlee55.alice-chan` to `com.yuna0x0.alice-chan`. The old id is
  not updated further.
- Model format is VRM 1.0, exported from VRoid Studio 2.14.0. Materials use `VRM10/MToon10`,
  facial data is VRM 1.0 Expressions, and physics is VRM 1.0 spring bones.
- Requires UniVRM 0.131.2 or newer (`com.vrmc.vrm` and `com.vrmc.gltf`). UniVRM is not declared
  as a UPM dependency; install it first.
- Minimum Unity version is 2022.3 LTS, matching UniVRM.
- The package ships the `.vrm` file and a prefab variant instead of extracted meshes, materials,
  textures and BlendShape clips. UniVRM imports the file and picks the built-in or URP MToon10
  shader for the project.
- The demo scene is a package sample, imported from the Package Manager window.

### Removed

- The FBX file is no longer inside the package. It is attached to each GitHub release.

## [1.0.4] - 2022-12-22

Last release under the old package id. See the
[release page](https://github.com/yuna0x0/AliceChan/releases/tag/v1.0.4).

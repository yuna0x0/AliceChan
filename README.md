# Alice Chan (Uniform ver.)

Alice Chan (Uniform ver.), a character made in VRoid Studio, distributed as a VRM 1.0 avatar.

![Alice Chan](https://user-images.githubusercontent.com/5277788/180695600-667c9f8f-abeb-41b3-87d9-4b9086332cf1.png)

## Getting the model

Each [release](https://github.com/yuna0x0/AliceChan/releases) has the model as VRM 1.0, as
VRM 0.x for applications that read only the older format, and as FBX with the mesh and rig
only, together with the Unity package as a unitypackage, a signed UPM tarball and a VPM zip.
The VRM files are also on [VRoid Hub](https://hub.vroid.com/en/characters/8722809851642259362/models/3746498118724430196)
and [itch.io](https://yuna0x0.itch.io/alice-chan).

## Unity

The Unity package needs Unity 2022.3 or newer and UniVRM 0.131.2 or newer (`com.vrmc.vrm`).
UniVRM is not declared as a package dependency, because a registry dependency breaks installs in
projects without the OpenUPM registry, so install it first. Four ways to install the package:

**OpenUPM**

```sh
openupm add com.vrmc.vrm com.yuna0x0.alice-chan
```

**Git URL**, in the Package Manager window under *Install package from git URL*, after
installing UniVRM:

```
https://github.com/yuna0x0/AliceChan.git?path=/unity/Packages/com.yuna0x0.alice-chan
```

**VPM**, with [ALCOM](https://vrc-get.anatawa12.com/en/alcom/) or
[vrc-get](https://github.com/vrc-get/vrc-get), after adding the listing
`https://vpm.yuna0x0.com/index.json`:

```sh
vrc-get repo add https://vpm.yuna0x0.com/index.json
vrc-get install com.yuna0x0.alice-chan
```

**unitypackage**, from the release page, imported into `Assets`.

The package contains `Models/AliceChan.vrm`, which UniVRM imports as a VRM 1.0 avatar with
`VRM10/MToon10` materials (built-in or URP, chosen by the project), and `Models/AliceChan.prefab`,
a prefab variant of the imported model. A demo scene is available as a sample in the Package
Manager window. To edit materials or expressions, use the Extract buttons on the `.vrm` importer.

## Source

The VRoid Studio project and the exports made from it are in `source/`. The package is authored
in the Unity project under `unity/`, and `basis/` holds the build of the avatar for Basis. [CONTRIBUTING.md](CONTRIBUTING.md) describes
how the files are regenerated and how a release is made.

## Other versions

- s&box (Source 2): [sbox-AliceChan](https://github.com/yuna0x0/sbox-AliceChan)

## License

This work by [yuna0x0](https://github.com/yuna0x0) is licensed under
[Creative Commons Attribution-ShareAlike 4.0 International](https://creativecommons.org/licenses/by-sa/4.0/).
See [LICENSE.md](LICENSE.md).

[![CC BY-SA 4.0](https://licensebuttons.net/l/by-sa/4.0/88x31.png)](https://creativecommons.org/licenses/by-sa/4.0/)

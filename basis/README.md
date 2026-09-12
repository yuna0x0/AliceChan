# Basis build

Builds the avatar as a Basis `.BEE` bundle from the VRM 1.0 package, using
[Watari](https://github.com/yuna0x0/watari-basis) to convert the VRM components into their Basis
equivalents and the Basis SDK to build and encrypt the bundle.

`pins.json` records the Basis commit, the Watari version and the UniVRM version the build uses.
Move the pins forward deliberately; Basis changes its editor version and SDK API between commits.

## Locally

```sh
uv run --project tools alicechan-basis-setup
uv run --project tools alicechan-basis-build --version 2.0.0 --targets windows,linux,android
```

Setup clones Basis at the pinned commit into `basis/.work/Basis` (ignored by git), unpacks
Watari into its `Packages` folder, and adds UniVRM, this repository's package and the build
package here to its manifest. Build runs the Basis project in batch mode with
`AliceChanBasisBuild.Build`, which converts the avatar and writes
`basis/.work/Basis/Basis/AssetBundles/AliceChan/`: the `.BEE` file, `dontuploadmepassword.txt`,
and a premade server config that carries the URL and password a client needs.

Every requested build target must be installed in the editor. Basis combines all targets into one
bundle in one process.

## In CI

`.github/workflows/basis.yml` runs the setup, then the build method inside a Unity editor image
through `game-ci/unity-builder`, then uploads the bundle and the config to Cloudflare R2 with
rclone. The secrets live in the `basis` environment.

The image comes from `.github/workflows/basis-image.yml`, which builds one editor image with the
windows-mono, linux-il2cpp and android modules using the GameCI CLI's `build-unity-image` and
pushes it to `ghcr.io/yuna0x0/unity-editor:<version>-basis`. GameCI's own images carry one
module each, and Basis builds every target in one editor process. Run that workflow once per
Unity version, before the first bundle build, and again whenever `pins.json` moves Basis to a
new editor version.

## The password

Basis encrypts the bundle; a client needs the password from the premade config to load it. For
this avatar the config is published with the release, since the avatar is CC BY-SA and meant to
be loaded by anyone.

## v0.2.1

Fix `ValidationPackageIndex.IdentityEquals` not respecting semver suffixes

## v0.2.0

- Add `SemVer` type model for a full semantic version representation including PreRelease and Build metadata with parsing and formatting functions
- Improve functions for getting semantic version (strings) from `ValidationPackageMetadata` and `ValidationPackageIndex` types based on new `SemVer` type
- Unify capitalization in Domain create functions

## v0.1.3

Add `BinaryContent` module to unify package content handling across downstream libraries

## v0.1.2

Add `PackageContentHash` module to unify package hash calculation across downstream libraries

## v0.1.1

Add `CQCHookEndpoint` field to `ValidationPackageMetadata`

## v0.1.0

Add support for in-package frontmatter bindings. Enables re-use of the frontmatter inside the package code

## v0.0.8
- Fix content hash being dependent on line endings (now, all content is normalized to \n before hashing)
- Fix code duplication in create functions for `ValidationPackageIndex`
- Unify `create` functions for Domain types

## v0.0.7

fix preview index download url

## v0.0.6

- Refactor and expose parsing & convenience functions:
  - Frontmatter
    - containsFrontmatter
    - tryExtractFromString
    - extractFromString
  - ValidationPackageMetadata
    - tryExtractFromString
    - extractFromString
    - tryExtractFromScript
    - extractFromScript

all frontmatter/metadata extraction functions will replace line endings with "\n", as YamlDotNet will replace any line endings with new line when presented the string anyways.

that way, the extracted frontmatter/metadata (especially field values, which caused problems due to YamlDotNet's default behavior) will be consistent across different platforms.

## v0.0.5

- Add `getPreviewIndex` function that downloads the currently released preview index from the github release.

## v0.0.4

- Replace line endings when parsing frontmatter

## v0.0.3

- Add create function to Author and OntologyAnnotation (https://github.com/nfdi4plants/arc-validate-package-registry/pull/27) 

## v0.0.2

- Add qol Domain functions (https://github.com/nfdi4plants/arc-validate-package-registry/pull/26)

## v0.0.1

- Initial release for AVPR API v1
# CKR - Unity Demo Project

*CKR* - demonstration unity project made within short time with those distinct features:
- Clicker mechanic. Ability to click for some arbitrary currency. Ability to autoclick, backend for support and scale.
- Reference-based async asset loading with Adressables.
- API async requests with in-demo views about weather, breeds with all needed framework for it. 

## Dependencies

- Unity 6.2+ (at least 6.0, async written in Awaitable)
- Zenject 9 (provided as Exjenct fork Plugin in repo)
- PrimeTween (provided as Plugin in repo)

## Distinct project structure keys

- Assets
  - Atlases - currently used UI atlases (one as master and possibly-to-remote, but used as master too due lack of directly atlas loader)
  - Scripts - module-based repo, each feature in each blob
  - Settings - Controllers for features, Unity for unity default settings (URP, etc)
  - Database - mainly currently used items data in demo.
 


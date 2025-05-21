# Changelog

All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).



## [1.0.1] - 2025-05-21

### Changed

- Adds new overloading methods **RegisterType** in class **ContainerBuilder**.
- **Binding** can be created with only one contract **Type** to improve performance.



## [1.0.0] - 2025-05-16
### Added

 - Attribute **Inject** supports optional binding that no excetion throw on no **Type** registration.
 - Attribute **Inject** supports name binding on field and property.
 - Attribute **Inject** supports parameter names binding on method.
 - Attribute **ConstructorInject** supports optional parameter binding that no excetion throw on no parameter **Type** registration.
 - Attribute **ConstructorInject** supports parameter names binding.
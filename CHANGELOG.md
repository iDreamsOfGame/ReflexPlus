# Changelog

All notable changes to this project will be documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).



## [1.0.7] - 2025-05-29

### Changed

- **ContainerBuilder** adds property **CreatedContainer** to hold last created container.



## [1.0.6] - 2025-05-23

### Changed

- Moves test cases to package folder.



## [1.0.4] - 2025-05-22

### Changed

- Changes the access modifier of class **ReflexPlusLogger** to **public**.



## [1.0.3] - 2025-05-22

### Removed

- Removed **ReflexPlusSettings**.



## [1.0.2] - 2025-05-21

### Changed

- Adds method **Unbind** in class **Container** to unbind **Type** binding.
- Adds mehtod **Unbind** in class **ContainerBuilder** to unbind **Type** binding before **Build**.



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
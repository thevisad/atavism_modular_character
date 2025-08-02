## 📦 Modular Character System Integration

This project uses the [`atavism_modular_character`](https://github.com/thevisad/atavism_modular_character) system as a Git submodule.

### 🔧 Cloning the Project with Submodules

To properly clone this repository including the modular character system:

```bash
git clone --recurse-submodules https://github.com/your/repo.git
```

If you've already cloned the repo, you can initialize the submodules manually:

```bash
git submodule update --init --recursive
```

### 📁 Submodule Location

The submodule is located at:

```
Assets/Dragonsan/ModularCustomizationSystem
```

### 🔄 Updating the Submodule

To pull the latest changes from the modular character submodule:

```bash
cd Assets/Dragonsan/ModularCustomizationSystem
git pull origin main
cd -
```

Then commit the updated submodule reference in the main repo:

```bash
git add Assets/Dragonsan/ModularCustomizationSystem
git commit -m "Update ModularCustomizationSystem submodule"
```

### ➕ Adding the Submodule (for maintainers)

To add the submodule (already done in this repo):

```bash
git submodule add https://github.com/thevisad/atavism_modular_character.git Assets/Dragonsan/ModularCustomizationSystem
```

---

This setup ensures any project using this repo always pulls the correct version of the modular character system.

# Conventional Commits Guide for MyCodeGent

## 📝 Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

## 🏷️ Types

### **Major Version Bump (Breaking Changes)**
```
BREAKING CHANGE: <description>
```
or
```
major: <description>
```

**Examples:**
```
BREAKING CHANGE: Remove support for .NET 8.0
major: Change API endpoint structure
feat!: Redesign entity model structure
```

### **Minor Version Bump (New Features)**
```
feat: <description>
feature: <description>
```

**Examples:**
```
feat: Add PostgreSQL database provider support
feat: Implement relationship management UI
feature: Add Git-based versioning system
```

### **Patch Version Bump (Bug Fixes & Improvements)**
```
fix: <description>
perf: <description>
refactor: <description>
style: <description>
docs: <description>
test: <description>
chore: <description>
```

**Examples:**
```
fix: Correct property spacing in Material UI
perf: Optimize code generation performance
refactor: Simplify version service logic
docs: Update README with new features
```

## 🎯 Scopes (Optional)

- `ui` - User interface changes
- `api` - API endpoint changes
- `core` - Core generation logic
- `templates` - Code templates
- `docs` - Documentation
- `config` - Configuration
- `deps` - Dependencies

**Examples:**
```
feat(ui): Add Material Design styling
fix(api): Correct version endpoint response
refactor(core): Simplify entity generation
```

## 🔢 Version Bumping Rules

### **Automatic Version Calculation:**

1. **BREAKING CHANGE** or **major:** → Bumps MAJOR version
   - `2.0.0` → `3.0.0`
   - Resets minor and patch to 0

2. **feat:** or **feature:** → Bumps MINOR version
   - `2.0.0` → `2.1.0`
   - Resets patch to 0

3. **fix:**, **perf:**, **refactor:**, etc. → Bumps PATCH version
   - `2.0.0` → `2.0.1`

4. **No conventional prefix** → No version bump
   - Version stays same, adds commit hash

## 📋 Examples

### **Breaking Change (Major)**
```bash
git commit -m "BREAKING CHANGE: Remove .NET 8.0 support

.NET 8.0 is no longer supported. Minimum version is now .NET 9.0.

BREAKING CHANGE: Minimum .NET version is now 9.0"
```
**Result:** `2.0.0` → `3.0.0`

### **New Feature (Minor)**
```bash
git commit -m "feat(ui): Add relationship management UI

- Add relationship tab for entities
- Support FK and navigation properties
- Visual relationship builder"
```
**Result:** `2.0.0` → `2.1.0`

### **Bug Fix (Patch)**
```bash
git commit -m "fix(ui): Correct property row spacing

Fixed Material Design spacing issues in property items.
Now uses 12px gap with proper grid layout."
```
**Result:** `2.0.0` → `2.0.1`

### **Multiple Commits**
```bash
# Commit 1
git commit -m "feat: Add Git versioning"
# Version: 2.0.0 → 2.1.0

# Commit 2
git commit -m "fix: Correct version display"
# Version: 2.1.0 → 2.1.1

# Commit 3
git commit -m "feat: Add Material Design UI"
# Version: 2.1.1 → 2.2.0
```

## 🏷️ Git Tags

### **Creating Release Tags**
```bash
# Tag current commit
git tag -a v2.1.0 -m "Release v2.1.0 - Material Design UI"

# Push tag to remote
git push origin v2.1.0
```

### **Version from Tag**
If on a tagged commit:
```
Version: 2.1.0
```

If commits after tag:
```
Version: 2.1.1-dev+abc1234
```

## 🔄 Semantic Versioning

### **Format:** `MAJOR.MINOR.PATCH[-PRERELEASE][+BUILD]`

**Examples:**
- `2.0.0` - Clean release
- `2.1.0-dev` - Development version
- `2.1.0+abc1234` - With commit hash
- `2.1.0-dev+abc1234` - Dev with commit hash
- `2.0.0-nogit` - No Git repository

### **Pre-release Identifiers:**
- `-dev` - Development/dirty working directory
- `-alpha` - Alpha release
- `-beta` - Beta release
- `-rc.1` - Release candidate

### **Build Metadata:**
- `+abc1234` - Short commit hash
- `+20241210` - Build date

## 📊 Version Display

### **In UI:**
```
Generator Version: 2.1.0-dev+abc1234
Git Branch: main
Git Commit: abc1234
Git Commit Message: feat: Add Material Design UI
Git Author: John Doe
Git Date: 2024-12-10
```

### **In API:**
```json
{
  "version": "2.1.0-dev+abc1234",
  "git": {
    "branch": "main",
    "commitHash": "abc1234",
    "commitMessage": "feat: Add Material Design UI",
    "commitAuthor": "John Doe",
    "commitDate": "2024-12-10T20:30:00",
    "isClean": false,
    "commitCount": 125
  }
}
```

## 🛠️ Development Workflow

### **Feature Development:**
```bash
# Create feature branch
git checkout -b feature/relationship-management

# Make commits with conventional format
git commit -m "feat(ui): Add relationship tab"
git commit -m "feat(core): Generate FK properties"
git commit -m "test: Add relationship tests"

# Merge to main
git checkout main
git merge feature/relationship-management

# Tag release
git tag -a v2.1.0 -m "Release v2.1.0"
git push origin v2.1.0
```

### **Hotfix:**
```bash
# Create hotfix branch
git checkout -b hotfix/spacing-issue

# Fix and commit
git commit -m "fix(ui): Correct property spacing"

# Merge and tag
git checkout main
git merge hotfix/spacing-issue
git tag -a v2.0.1 -m "Hotfix v2.0.1"
git push origin v2.0.1
```

## 📈 Benefits

### **Automated Versioning:**
- ✅ No manual version updates
- ✅ Consistent versioning
- ✅ Clear version history
- ✅ Semantic meaning

### **Clear History:**
- ✅ Understand changes at a glance
- ✅ Generate changelogs automatically
- ✅ Track breaking changes
- ✅ Know when features were added

### **Better Collaboration:**
- ✅ Team knows impact of changes
- ✅ Clear commit messages
- ✅ Easy code review
- ✅ Professional workflow

## 🎓 Quick Reference

| Type | Version Bump | Example |
|------|--------------|---------|
| `BREAKING CHANGE:` | MAJOR | `2.0.0` → `3.0.0` |
| `major:` | MAJOR | `2.0.0` → `3.0.0` |
| `feat:` | MINOR | `2.0.0` → `2.1.0` |
| `feature:` | MINOR | `2.0.0` → `2.1.0` |
| `fix:` | PATCH | `2.0.0` → `2.0.1` |
| `perf:` | PATCH | `2.0.0` → `2.0.1` |
| `refactor:` | PATCH | `2.0.0` → `2.0.1` |
| `docs:` | PATCH | `2.0.0` → `2.0.1` |
| `style:` | PATCH | `2.0.0` → `2.0.1` |
| `test:` | PATCH | `2.0.0` → `2.0.1` |
| `chore:` | PATCH | `2.0.0` → `2.0.1` |
| No prefix | No bump | `2.0.0+abc1234` |

---

**Follow these conventions for automatic, semantic versioning!** 🚀

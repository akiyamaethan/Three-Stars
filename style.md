# Three Stars – Code & Asset Style Guide

This document defines the **coding, UI, texture, and audio conventions** for *Three Stars*. All code and assets committed to the GitHub repository **must follow these rules** to ensure consistency, readability, and smooth collaboration.

This style guide is mandatory for all contributors. Code or assets that do not follow these conventions may be rejected during review.


---

## 1. Indentation & Spacing

- Use **spaces only**, not tabs
- **4 spaces per indentation level**
- One statement per line
- No trailing whitespace
- One blank line between logical blocks of code

**Example:**
```csharp
if (isCooking)
{
    StartTimer();
}
```

---

# 2. Naming Conventions

#  Classes & Scripts
- **PascalCase**
- File name **must match** class name

```csharp
public class OrderManager : MonoBehaviour
```

# Methods
- **PascalCase**
- Use clear verb phrases

```csharp
void SpawnCustomer()
void UpdateScore()
```

# Variables
- **camelCase** for local variables and parameters
- **_camelCase** for private fields
- **PascalCase** for public fields (Unity Inspector)

```csharp
[SerializeField] private int _maxOrders;
public float CookTime;
```

# Constants & Enums
- **PascalCase**

```csharp
public const int MaxStars = 3;
```

---

## 3. Brace Styling

- Use **Allman style** braces
- Opening brace on a **new line**

```csharp
public void CompleteOrder()
{
    score++;
}
```

---

# 4. Formula & Expression Formatting

- Use parentheses to improve clarity
- Avoid deeply nested expressions
- Extract complex logic into helper methods

```csharp
float finalScore = baseScore * (comboMultiplier + bonus);
```

**Avoid:**
```csharp
score = a*b+c-d/e*f;
```

---

# 5. Code Readability & Structure

## Line Length
- Maximum **100 characters per line**

## Comments
- Use comments to explain **why**, not what
- Avoid obvious comments

```csharp
// Prevent soft-lock if customer leaves mid-order
ResetOrderState();
```

## Method Size
- Functions should generally be **20–40 lines max**
- Break large systems into multiple components

##  Script Responsibility
- One script = **one clear responsibility**
- Avoid "god scripts"

---

## 6. Language & Engine Standards

- Language: **C# 10.0** (Unity default)
- Engine: **Unity 2022+ LTS**
- Use Unity APIs and patterns where applicable
- Avoid unsafe code and reflection unless required

---

# Extra Credit – Technical Style Guidelines

## 7. Asset Naming Conventions

### General Rules
- Use **snake_case**
- No spaces
- Include asset type prefix

### Prefixes
- `spr_` – Sprites
- `ui_` – UI elements
- `sfx_` – Sound effects
- `bgm_` – Music
- `mat_` – Materials

**Examples:**
```
spr_customer_idle.png
ui_star_filled.png
sfx_order_complete.wav
```

---

## 8. Texture Resolution & Scaling

- Pixel-art scale: **1 unit = 16 pixels**
- UI sprites: multiples of **16x16** or **32x32**
- No non-uniform scaling in scenes
- Use Sprite Pixels Per Unit consistently

---

## 9. UI Style Guide

## Interaction States
- Default
- Hover (lighter or highlighted)
- Pressed (darker or depressed)
- Disabled (desaturated)

## Visual Rules
- Consistent padding and margins
- Avoid visual noise
- UI animations should be **short and readable**

## Color Usage
- Stars: gold/yellow
- Fail states: red
- Neutral UI: off-white / gray
- Colors may be shaded differently but overarching decisions will be made final

---

## 10. Audio Format Specifications

## Guidelines
- Normalize audio
- No clipping
- Keep SFX short and punchy

---

## 11. Enforcement

- Code reviews must check style compliance
- Non-compliant code may be rejected

---

**Last Updated: line**


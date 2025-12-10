# Advanced Relationships & Constraints System - Implementation Plan

## 🎯 **Overview**

Implementing a comprehensive system for:
1. **Property Constraints** - Advanced validation rules
2. **Foreign Keys** - Database relationships
3. **Business Keys** - Alternate/composite keys
4. **Visual DB Diagram** - Interactive relationship viewer

---

## 📊 **Phase 1: Property Constraints**

### **String Constraints**
- ✅ MaxLength (already implemented)
- ⬜ MinLength
- ⬜ Regex Pattern
- ⬜ Default Value
- ⬜ Unique Index

### **Numeric Constraints**
- ⬜ Min Value (Range)
- ⬜ Max Value (Range)
- ⬜ Default Value
- ⬜ Precision/Scale (for decimals)

### **General Constraints**
- ⬜ Required (already implemented)
- ⬜ Unique
- ⬜ Index
- ⬜ Default Value
- ⬜ Computed Column

### **UI Implementation**
```
Property Row:
[Name] [Type] [Key] [Required] [⚙️ Constraints] [🔗 Relationship] [×]

Constraints Modal:
┌─────────────────────────────────┐
│ Property Constraints            │
├─────────────────────────────────┤
│ String Constraints:             │
│ Min Length: [___]               │
│ Max Length: [___]               │
│ Regex: [___________]            │
│ Default: [___________]          │
│                                 │
│ Numeric Constraints:            │
│ Min Value: [___]                │
│ Max Value: [___]                │
│ Precision: [__] Scale: [__]     │
│                                 │
│ Index Options:                  │
│ ☐ Unique                        │
│ ☐ Create Index                  │
│ ☐ Computed Column               │
│                                 │
│ [Cancel] [Apply]                │
└─────────────────────────────────┘
```

---

## 🔗 **Phase 2: Relationships & Keys**

### **Foreign Key Configuration**
```
Relationship Modal:
┌─────────────────────────────────┐
│ Configure Relationship          │
├─────────────────────────────────┤
│ Relationship Type:              │
│ ○ One-to-Many                   │
│ ○ Many-to-One                   │
│ ○ Many-to-Many                  │
│ ○ One-to-One                    │
│                                 │
│ Related Entity: [Dropdown ▼]   │
│ Foreign Key Property: [____]    │
│ Principal Key: [Dropdown ▼]     │
│                                 │
│ Navigation Properties:          │
│ This Entity: [___________]      │
│ Related Entity: [___________]   │
│                                 │
│ Cascade Options:                │
│ On Delete: [Cascade ▼]          │
│ On Update: [Cascade ▼]          │
│                                 │
│ [Cancel] [Create Relationship]  │
└─────────────────────────────────┘
```

### **Business Keys (Alternate Keys)**
```
Business Key Configuration:
┌─────────────────────────────────┐
│ Business Keys                   │
├─────────────────────────────────┤
│ Existing Keys:                  │
│ • Email (Unique)                │
│ • Username (Unique)             │
│ • (OrderNumber, Year) Composite │
│                                 │
│ Create New Business Key:        │
│ Key Name: [___________]         │
│                                 │
│ Properties:                     │
│ ☑ Email                         │
│ ☐ Username                      │
│ ☐ PhoneNumber                   │
│                                 │
│ [Add Business Key]              │
└─────────────────────────────────┘
```

### **Relationship Types**

#### **One-to-Many**
```
Customer (1) ──< (∞) Order
- Customer has many Orders
- Order belongs to one Customer
- FK: Order.CustomerId → Customer.Id
```

#### **Many-to-Many**
```
Student (∞) ──< >── (∞) Course
- Student has many Courses
- Course has many Students
- Join Table: StudentCourse
```

#### **One-to-One**
```
User (1) ──── (1) Profile
- User has one Profile
- Profile belongs to one User
- FK: Profile.UserId → User.Id (Unique)
```

---

## 📐 **Phase 3: Visual DB Diagram**

### **Diagram Features**

#### **Entity Boxes**
```
┌─────────────────┐
│ Customer        │ ← Entity Name
├─────────────────┤
│ 🔑 Id           │ ← Primary Key
│ 📧 Email        │ ← Business Key
│   Name          │
│   Phone         │
│ 📅 CreatedAt    │
└─────────────────┘
```

#### **Relationship Lines**
```
Customer ────────< Order
    1              ∞

Order >────────< Product
  ∞              ∞
  
User ──────────── Profile
 1                1
```

#### **Cardinality Indicators**
- `1` - One
- `∞` - Many
- `0..1` - Zero or One
- `1..*` - One or More

### **Diagram UI**
```
┌─────────────────────────────────────────────┐
│ 📊 Database Diagram        [−] [□] [×]      │
├─────────────────────────────────────────────┤
│                                             │
│  ┌──────────┐         ┌──────────┐         │
│  │Customer  │────────<│Order     │         │
│  │🔑Id      │    1  ∞ │🔑Id      │         │
│  │Email     │         │CustomerId│         │
│  │Name      │         │Total     │         │
│  └──────────┘         └──────────┘         │
│                            │                │
│                            │ ∞              │
│                            │                │
│                       ┌────┴─────┐          │
│                       │OrderItem │          │
│                       │🔑Id      │          │
│                       │OrderId   │          │
│                       │ProductId │          │
│                       └──────────┘          │
│                            │                │
│                            │ ∞              │
│                            │                │
│                       ┌────┴─────┐          │
│                       │Product   │          │
│                       │🔑Id      │          │
│                       │Name      │          │
│                       │Price     │          │
│                       └──────────┘          │
│                                             │
│ [Auto Layout] [Zoom In] [Zoom Out] [Reset] │
└─────────────────────────────────────────────┘
```

### **Diagram Interactions**
- **Click Entity** - Highlight and show details
- **Click Relationship** - Edit relationship
- **Drag Entity** - Reposition
- **Double-Click** - Edit entity
- **Right-Click** - Context menu
- **Minimize** - Collapse to corner icon
- **Maximize** - Expand to full view

---

## 💾 **Data Model Updates**

### **Property Model Extension**
```javascript
{
    name: "Email",
    type: "string",
    isKey: false,
    isRequired: true,
    
    // NEW: Constraints
    constraints: {
        minLength: 5,
        maxLength: 100,
        regex: "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$",
        defaultValue: null,
        unique: true,
        indexed: true,
        precision: null,  // for decimal
        scale: null       // for decimal
    },
    
    // NEW: Relationship
    relationship: {
        type: "ForeignKey",  // or null
        relatedEntity: "Customer",
        relatedProperty: "Id",
        navigationProperty: "Customer",
        inverseNavigationProperty: "Orders",
        onDelete: "Cascade",  // Cascade, SetNull, Restrict
        onUpdate: "Cascade"
    }
}
```

### **Entity Model Extension**
```javascript
{
    id: 1,
    name: "Order",
    hasAuditFields: true,
    hasSoftDelete: true,
    properties: [...],
    
    // NEW: Business Keys
    businessKeys: [
        {
            name: "OrderNumber",
            properties: ["OrderNumber", "Year"],
            isUnique: true
        }
    ],
    
    // NEW: Relationships
    relationships: [
        {
            type: "OneToMany",
            relatedEntity: "OrderItem",
            foreignKey: "OrderId",
            principalKey: "Id",
            navigationProperty: "OrderItems",
            inverseNavigationProperty: "Order"
        }
    ],
    
    // NEW: Diagram Position
    diagramPosition: {
        x: 100,
        y: 100
    }
}
```

---

## 🎨 **UI Components to Add**

### **1. Property Constraints Button**
```html
<button class="btn-icon" onclick="showPropertyConstraints(entityId, propIdx)">
    ⚙️
</button>
```

### **2. Relationship Button**
```html
<button class="btn-icon" onclick="showRelationshipModal(entityId, propIdx)">
    🔗
</button>
```

### **3. Business Keys Tab**
```html
<div class="entity-section">
    <h4>🔑 Business Keys</h4>
    <div class="business-keys-list">
        <!-- Business keys here -->
    </div>
    <button onclick="addBusinessKey(entityId)">+ Add Business Key</button>
</div>
```

### **4. Diagram Panel**
```html
<div class="diagram-panel minimized" id="diagramPanel">
    <div class="diagram-header">
        <h3>📊 Database Diagram</h3>
        <button onclick="toggleDiagram()">−</button>
    </div>
    <div class="diagram-canvas">
        <svg id="diagramSvg"></svg>
    </div>
</div>
```

---

## 🔧 **Implementation Steps**

### **Step 1: Property Constraints Modal** ✅ Ready to implement
- Create modal HTML
- Add constraint form fields
- Update property model
- Save/load constraints

### **Step 2: Relationship Configuration** ⬜ Next
- Create relationship modal
- Entity dropdown
- Relationship type selector
- FK/Navigation property inputs
- Cascade options

### **Step 3: Business Keys** ⬜ Planned
- Business key manager
- Multi-property selection
- Unique constraint handling

### **Step 4: Visual Diagram** ⬜ Planned
- SVG canvas setup
- Entity box rendering
- Relationship line drawing
- Drag & drop positioning
- Minimize/maximize functionality

### **Step 5: Backend Integration** ⬜ Planned
- Update GenerationConfig model
- Update EntityModel
- Update templates to generate:
  - Fluent API configurations
  - Foreign key properties
  - Navigation properties
  - Indexes
  - Constraints
  - Alternate keys

---

## 📊 **Benefits**

### **For Users:**
✅ **Visual Understanding** - See relationships at a glance
✅ **Comprehensive Constraints** - All validation rules
✅ **Professional Output** - Production-ready code
✅ **No Manual Configuration** - Everything in UI

### **For Generated Code:**
✅ **Proper Relationships** - FK constraints
✅ **Navigation Properties** - Easy querying
✅ **Indexes** - Performance optimization
✅ **Validation** - Data integrity
✅ **Business Rules** - Alternate keys

### **For Maintenance:**
✅ **Clear Documentation** - Visual diagram
✅ **Easy Updates** - Edit relationships visually
✅ **No Errors** - Validated configurations
✅ **Professional** - Enterprise-grade

---

## 🚀 **Next Steps**

**Ready to implement?** I can start with:

1. **Property Constraints Modal** - Full constraint editor
2. **Relationship Configuration** - FK and navigation properties
3. **Visual DB Diagram** - Interactive SVG diagram
4. **Business Keys** - Alternate key management

**This will make your code generator truly enterprise-ready!** 🎯

Would you like me to proceed with the implementation?

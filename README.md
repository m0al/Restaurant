# Restaurant Management System


<p align="center">
  A complete restaurant management system built using <b>C#</b>, <b>.NET Framework</b>, and <b>Microsoft SQL Server</b>.  
  Designed as a full role-based system for Admins, Managers, Chefs, Reservation Coordinators, and Customers.
</p>

---

## Overview

FoodiePoint is a restaurant management system built to improve workflow efficiency and elevate customer experience.  
The system operates on a **multi-role structure** with different permissions and UI flows for each user type:

- **Administrator (Universal)**
- **Manager**
- **Chef**
- **Reservation Coordinator**
- **Customer**

All roles interact with a **central SQL database**, ensuring clean data structure, scalability, and consistency.

---

## Technologies Used

- **C# (.NET Framework)**
- **Microsoft SQL Server**
- **Windows Forms (WinForms)**
- **SMTP / Mailgun API (Password Reset)**
- **Object-Oriented Programming (OOP)**  
  (Classes, inheritance, helper utilities, session management)

---

## Color Palette

| Element | Color |
|--------|--------|
| Primary | `#7c3aed` |
| Background | `#0b132b` |
| Text | `#e6edf3` |
| Muted | `#9aa4b2` |
| Line | `rgba(230,237,243,0.08)` |
| Hover | `rgba(124,58,237,0.16)` |

---

## System Features (Role-Based)

### **Admin**
- Manage all users  
- View sales reports  
- View customer feedback  
- Full database access  

### **Chef**
- Manage inventory  
- View and update order history  
- Update active orders  
- Mark orders as In Progress / Completed  

### **Manager**
- Manage restaurant halls  
- Manage menu items  
- View reservation reports  
- Approve, update, or deactivate halls  

### **Reservation Coordinator**
- Manage reservations  
- Review and approve requests  
- Update reservation statuses  
- Access reservation history  

### **Customer**
- Order food  
- View and filter order status  
- Manage reservation status  
- Send feedback  
- Send reservation-related requests  

---

## System Architecture

### **Core OOP Classes**
- `adminClass`
- `managerClass`
- `chefClass`
- `reservationCoordinatorClass`
- `customerClass`
- `foodieLoginClass`
- `foodieForgotPasswordClass`
- `foodieDashboardClass`
- `formHelper`  
- `UserSession`

---

## Running the Project

#### 1. Open in Visual Studio  
#### 2. Ensure SQL Server database is configured  
#### 3. Run using the button above

⸻

Building for Production

dotnet build
dotnet publish -c Release


⸻

## Screenshots


<p align="center">
  <img src="Screenshot 2025-12-10 at 3.03.41 PM" width="800">
</p>

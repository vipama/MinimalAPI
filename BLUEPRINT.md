# MinimalAPI Blueprint

## 1) Project Overview
`MinimalAPI` เป็นโปรเจกต์ตัวอย่าง ASP.NET Core Minimal API แยกเป็น 2 โปรเจกต์หลัก:
- `MyApi.Web` สำหรับ HTTP API
- `MyApi.Data` สำหรับ Data Access ด้วย Entity Framework Core

สถาปัตยกรรมโดยรวม:
- Client -> `MyApi.Web` endpoints
- `MyApi.Web` inject `AppDataContexts`
- `AppDataContexts` เชื่อมต่อ MySQL ผ่าน `Pomelo.EntityFrameworkCore.MySql`

## 2) Solution Structure
- `MinimalAPI.sln`
- `MyApi.Web/`
  - `Program.cs` กำหนด service, middleware, และ endpoint
  - `appsettings.json` / `appsettings.Development.json` เก็บค่า config
- `MyApi.Data/`
  - `AppDataContexts.cs` ประกาศ `DbContext` และ Entity (`Patient`, `Doctor`)
  - `DesignTimeDbContextFactory.cs` ใช้สำหรับ EF tooling (migrations)
  - `Migrations/` ไฟล์ migration และ snapshot

## 3) Tech Stack
- .NET `net10.0`
- ASP.NET Core Minimal API
- Entity Framework Core `9.0.0`
- Pomelo MySQL Provider `9.0.0`
- OpenAPI (`Microsoft.AspNetCore.OpenApi`)

## 4) Data Model
ตารางหลัก:
- `Patient`
  - `Id: long`
  - `Name: string`
  - `LastName: string`
- `Doctor`
  - `Id: long`
  - `DoctorName: string`
  - `DepartmentName: string`

DbContext:
- `AppDataContexts`
  - `DbSet<Patient> Patients`
  - `DbSet<Doctor> Doctors`

## 5) API Endpoints
General:
- `GET /weatherforecast`

Patient:
- `GET /patients` รายการคนไข้ทั้งหมด
- `GET /patients/{id}` คนไข้รายบุคคล
- `POST /patients` เพิ่มคนไข้
- `PUT /patients/{id}` แก้ไขคนไข้
- `DELETE /Delpatients/{id}` ลบคนไข้

Doctor:
- `GET /doctors` รายการหมอทั้งหมด
- `GET /doctors/{id}` หมอรายบุคคล
- `POST /doctors` เพิ่มหมอ
- `PUT /doctors/{id}` แก้ไขหมอ
- `DELETE /Deldoctors/{id}` ลบหมอ

## 6) Runtime Configuration
ใน `MyApi.Web/Program.cs` มีการลงทะเบียน DbContext:
- อ่าน connection string ชื่อ `DefaultConnection`
- ใช้ `ServerVersion.AutoDetect(...)` ตอน runtime

หมายเหตุ:
- มีโค้ดตัวอย่าง `ForwardedHeaders` สำหรับ deploy หลัง Nginx (ยังคอมเมนต์อยู่)
- ใน `Development` จะเปิด `app.MapOpenApi()`

## 7) Design-Time (Migrations)
`MyApi.Data/DesignTimeDbContextFactory.cs` ใช้ connection string แบบ fixed เพื่อให้ EF tooling ทำงานได้โดยไม่ต้องพึ่ง `AutoDetect` จาก DB runtime

Workflow ที่ใช้บ่อย:
1. สร้าง migration
2. อัปเดตฐานข้อมูล
3. รัน `MyApi.Web`

## 8) Suggested Next Improvements
- ปรับ route ลบข้อมูลให้เป็น REST convention เช่น `DELETE /patients/{id}` และ `DELETE /doctors/{id}`
- เพิ่ม validation สำหรับ request body
- เพิ่ม global error handling
- เพิ่ม versioning ของ API (ถ้าจะใช้งานระยะยาว)
- แยก endpoint groups / modules เพื่อดูแลง่ายขึ้น

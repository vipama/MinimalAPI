namespace MyApi.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
public class AppDataContexts : DbContext
{
    public AppDataContexts(DbContextOptions<AppDataContexts> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
}

// ปรับ Class ให้มี Property ตรงกับ Column ในรูป
[Table("Patient")] // ระบุชื่อตารางให้ตรงกับในรูปเป๊ะๆ
public class Patient
{
    public long Id { get; set; } // bigint(20) ใน C# คือ long
    public string Name { get; set; } = string.Empty; // varchar(100)
    public string LastName { get; set; } = string.Empty; // varchar(100)
}

[Table("Doctor")]
public class Doctor
{
     public long Id { get; set; } // bigint(20) ใน C# คือ long
     public string DoctorName { get; set; } = string.Empty; // varchar(100)
     public string DepartmentName { get; set; } = string.Empty; // varchar(100)
}

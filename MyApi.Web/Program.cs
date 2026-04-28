using MyApi.Data; // 1. เพิ่ม using namespace ของโปรเจกต์ Data
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.HttpOverrides;  // เอาไว้ใช้กับ Nginx ใน linux

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// สร้างการเชื่อมต่อกับฐานข้อมูล
// ตัวอย่างการตั้งค่าใน Program.cs
builder.Services.AddDbContext<AppDataContexts>(options =>
    options.UseMySql( // หรือ .UseSqlite / .UseSqlServer
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

var app = builder.Build();

// #############################################################################################
// เพิ่มบรรทัดนี้ก่อนบรรทัดอื่นๆ ที่เกี่ยวกับ Routing หรือ Auth
// app.UseForwardedHeaders(new ForwardedHeadersOptions
// {
//     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
// });

// app.MapGet("/", () => "Hello via Nginx Reverse Proxy!");
// #############################################################################################


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/patients", async (AppDataContexts db) =>
    await db.Patients.ToListAsync());

app.MapGet("/doctors", async (AppDataContexts db) =>
    await db.Doctors.ToListAsync());

app.MapPut("/doctors/{id}", async (long id, Doctor inputDoctor, AppDataContexts db) =>
{
    var doctor = await db.Doctors.FindAsync(id);

    if (doctor is null) return Results.NotFound();

    doctor.DoctorName = inputDoctor.DoctorName;
    doctor.DepartmentName = inputDoctor.DepartmentName;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPut("/patients/{id}", async (long id, Patient inputPatient, AppDataContexts db) =>
{
    // 1. ค้นหาข้อมูลคนไข้เดิมในฐานข้อมูลตาม ID ที่ส่งมา
    var patient = await db.Patients.FindAsync(id);

    // 2. ถ้าไม่เจอ ให้ส่งสถานะ 404 Not Found กลับไป
    if (patient is null) return Results.NotFound();

    // 3. นำค่าใหม่ที่ส่งมา (inputPatient) ไปทับค่าเดิม
    patient.Name = inputPatient.Name;
    patient.LastName = inputPatient.LastName;

    // 4. บันทึกการเปลี่ยนแปลงลงฐานข้อมูล
    await db.SaveChangesAsync();

    // 5. ส่งสถานะ 204 No Content (แก้ไขสำเร็จแต่ไม่ต้องส่งข้อมูลกลับ) 
    // หรือจะส่ง Results.Ok(patient) ก็ได้ครับ
    return Results.NoContent();
});
// 2. GET: ดึงข้อมูลคนไข้รายบุคคลตาม ID (Read One)
app.MapGet("/patients/{id}", async (long id, AppDataContexts db) =>
    await db.Patients.FindAsync(id) is Patient patient
        ? Results.Ok(patient)
        : Results.NotFound())
    .WithName("GetPatientById");

app.MapGet("/doctors/{id}", async (long id, AppDataContexts db) =>
    await db.Doctors.FindAsync(id) is Doctor doctor
        ? Results.Ok(doctor)
        : Results.NotFound())
    .WithName("GetDoctorById");

// 3. POST: เพิ่มคนไข้ใหม่ (Create)
app.MapPost("/patients", async (Patient patient, AppDataContexts db) =>
{
    db.Patients.Add(patient);
    await db.SaveChangesAsync();
    
    // ส่งกลับเป็น 201 Created พร้อมบอก URL ที่จะเข้าถึงข้อมูลนี้ได้
    return Results.Created($"/patients/{patient.Id}", patient);
})
.WithName("CreatePatient");   

app.MapPost("/doctors", async (Doctor doctor, AppDataContexts db) =>
{
    db.Doctors.Add(doctor);
    await db.SaveChangesAsync();

    return Results.Created($"/doctors/{doctor.Id}", doctor);
})
.WithName("CreateDoctor");

// 6. DELETE: ลบข้อมูลคนไข้ (Delete)
app.MapDelete("/Delpatients/{id}", async (long id, AppDataContexts db) =>
{
    if (await db.Patients.FindAsync(id) is Patient patient)
    {
        db.Patients.Remove(patient);
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Deleted successfully", id = id });
    }

    return Results.NotFound();
})
.WithName("DeletePatient");

app.MapDelete("/Deldoctors/{id}", async (long id, AppDataContexts db) =>
{
    if (await db.Doctors.FindAsync(id) is Doctor doctor)
    {
        db.Doctors.Remove(doctor);
        await db.SaveChangesAsync();
        return Results.Ok(new { message = "Deleted successfully", id = id });
    }

    return Results.NotFound();
})
.WithName("DeleteDoctor");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

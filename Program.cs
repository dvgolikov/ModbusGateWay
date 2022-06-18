using ModbusGateWay;
using ModbusGateWay.Modbus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<DataCollector>();


var app = builder.Build();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}");
app.MapFallbackToController("DefaultPath", "Modbus");

app.Run();

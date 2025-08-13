using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeuProjetoBlazorServer.Data;
using MeuProjetoBlazorServer.Services;
using System.Linq;

namespace MeuProjetoBlazorServer.Controllers
{
    [ApiController]
    [Route("api/gas")]
    public class GasController : ControllerBase
    {
        private readonly IFuelRecordService _fuelRecordService;

        public GasController(IFuelRecordService fuelRecordService)
        {
            _fuelRecordService = fuelRecordService;
        }

        // GET: api/gas - Lista todos os registros de combustível
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FuelRecord>>> GetFuelRecords()
        {
            try
            {
                var fuelRecords = await _fuelRecordService.GetAllFuelRecordsAsync();
                return Ok(fuelRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // GET: api/gas/5 - Busca registro por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<FuelRecord>> GetFuelRecord(int id)
        {
            try
            {
                var fuelRecord = await _fuelRecordService.GetFuelRecordByIdAsync(id);
                
                if (fuelRecord == null)
                {
                    return NotFound($"Registro de combustível com ID {id} não encontrado.");
                }

                return Ok(fuelRecord);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // GET: api/gas/vehicle/5 - Busca registros por veículo
        [HttpGet("vehicle/{vehicleId}")]
        public async Task<ActionResult<IEnumerable<FuelRecord>>> GetFuelRecordsByVehicle(int vehicleId)
        {
            try
            {
                var fuelRecords = await _fuelRecordService.GetFuelRecordsByVehicleAsync(vehicleId);
                return Ok(fuelRecords);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // POST: api/gas - Cria novo registro
        [HttpPost]
        public async Task<ActionResult<FuelRecord>> CreateFuelRecord(FuelRecord fuelRecord)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdFuelRecord = await _fuelRecordService.CreateFuelRecordAsync(fuelRecord);
                return CreatedAtAction(nameof(GetFuelRecord), new { id = createdFuelRecord.Id }, createdFuelRecord);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // PUT: api/gas/5 - Atualiza registro existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFuelRecord(int id, FuelRecord fuelRecord)
        {
            try
            {
                if (id != fuelRecord.Id)
                {
                    return BadRequest("ID do registro não corresponde.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedFuelRecord = await _fuelRecordService.UpdateFuelRecordAsync(fuelRecord);
                return Ok(updatedFuelRecord);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // DELETE: api/gas/5 - Deleta registro (soft delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFuelRecord(int id)
        {
            try
            {
                var result = await _fuelRecordService.DeleteFuelRecordAsync(id);
                
                if (!result)
                {
                    return NotFound($"Registro de combustível com ID {id} não encontrado.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        // GET: api/gas/statistics/vehicle - Estatísticas por veículo
        [HttpGet("statistics/vehicle")]
        public async Task<ActionResult<IEnumerable<VehicleStatistics>>> GetVehicleStatistics()
        {
            try
            {
                var fuelRecords = await _fuelRecordService.GetAllFuelRecordsAsync();
                var vehicles = await _fuelRecordService.GetAllVehiclesAsync();
                
                var statistics = new List<VehicleStatistics>();
                
                foreach (var vehicle in vehicles.Where(v => !v.IsDeleted))
                {
                    var vehicleRecords = fuelRecords
                        .Where(fr => fr.VehicleId == vehicle.Id && !fr.IsDeleted)
                        .OrderBy(fr => fr.CreatedAt)
                        .ToList();
                    
                    if (vehicleRecords.Count == 0) continue;
                    
                    var stats = CalculateVehicleStatistics(vehicle, vehicleRecords);
                    statistics.Add(stats);
                }
                
                // Ordena por consumo médio decrescente (mais eficiente primeiro)
                return Ok(statistics.OrderByDescending(s => s.AverageConsumption));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        private VehicleStatistics CalculateVehicleStatistics(Vehicle vehicle, List<FuelRecord> records)
        {
            var stats = new VehicleStatistics
            {
                VehicleId = vehicle.Id,
                VehicleName = vehicle.Name,
                VehicleYear = vehicle.Year,
                TotalFuelRecords = records.Count,
                TotalFuelConsumed = records.Sum(r => r.FuelAmount),
                TotalCost = records.Sum(r => r.TotalCost),
                AverageFuelPrice = records.Average(r => r.FuelPricePerUnit)
            };

            // Calcula consumo médio entre tanques cheios
            var fullTankRecords = records.Where(r => r.FullTank).OrderBy(r => r.CreatedAt).ToList();
            
            if (fullTankRecords.Count >= 2)
            {
                var consumptionPeriods = new List<ConsumptionPeriod>();
                
                for (int i = 0; i < fullTankRecords.Count - 1; i++)
                {
                    var current = fullTankRecords[i];
                    var next = fullTankRecords[i + 1];
                    
                    // Busca todos os registros entre os dois tanques cheios
                    var recordsBetween = records
                        .Where(r => r.CreatedAt > current.CreatedAt && r.CreatedAt <= next.CreatedAt)
                        .OrderBy(r => r.CreatedAt)
                        .ToList();
                    
                    var distance = next.Odometer - current.Odometer;
                    var fuelConsumed = recordsBetween.Sum(r => r.FuelAmount);
                    
                    if (distance > 0 && fuelConsumed > 0)
                    {
                        consumptionPeriods.Add(new ConsumptionPeriod
                        {
                            StartDate = current.CreatedAt,
                            EndDate = next.CreatedAt,
                            Distance = distance,
                            FuelConsumed = fuelConsumed,
                            Consumption = distance / fuelConsumed
                        });
                    }
                }
                
                if (consumptionPeriods.Count > 0)
                {
                    stats.ConsumptionPeriods = consumptionPeriods;
                    stats.AverageConsumption = consumptionPeriods.Average(p => p.Consumption);
                    stats.TotalDistance = consumptionPeriods.Sum(p => p.Distance);
                    stats.TotalFuelBetweenFullTanks = consumptionPeriods.Sum(p => p.FuelConsumed);
                }
            }
            
            // Se não há períodos entre tanques cheios, calcula baseado em toda a quilometragem
            if (stats.ConsumptionPeriods.Count == 0 && records.Count > 1)
            {
                var minOdometer = records.Min(r => r.Odometer);
                var maxOdometer = records.Max(r => r.Odometer);
                var totalDistance = maxOdometer - minOdometer;
                var totalFuel = records.Sum(r => r.FuelAmount);
                
                if (totalDistance > 0 && totalFuel > 0)
                {
                    stats.AverageConsumption = totalDistance / totalFuel;
                    stats.TotalDistance = totalDistance;
                }
            }
            
            return stats;
        }
    }

    public class VehicleStatistics
    {
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public int VehicleYear { get; set; }
        public int TotalFuelRecords { get; set; }
        public decimal TotalFuelConsumed { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AverageFuelPrice { get; set; }
        public decimal AverageConsumption { get; set; } // km/L
        public decimal TotalDistance { get; set; }
        public decimal TotalFuelBetweenFullTanks { get; set; }
        public List<ConsumptionPeriod> ConsumptionPeriods { get; set; } = new List<ConsumptionPeriod>();
    }

    public class ConsumptionPeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Distance { get; set; }
        public decimal FuelConsumed { get; set; }
        public decimal Consumption { get; set; } // km/L
    }
}

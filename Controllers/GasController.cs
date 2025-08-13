using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeuProjetoBlazorServer.Data;
using MeuProjetoBlazorServer.Services;

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
    }
}

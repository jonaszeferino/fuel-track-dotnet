using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MeuProjetoBlazorServer.Data;

namespace MeuProjetoBlazorServer.Services
{
    public interface IFuelRecordService
    {
        Task<List<FuelRecord>> GetAllFuelRecordsAsync();
        Task<FuelRecord?> GetFuelRecordByIdAsync(int id);
        Task<List<FuelRecord>> GetFuelRecordsByVehicleAsync(int vehicleId);
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<FuelRecord> CreateFuelRecordAsync(FuelRecord fuelRecord);
        Task<FuelRecord> UpdateFuelRecordAsync(FuelRecord fuelRecord);
        Task<bool> DeleteFuelRecordAsync(int id);
    }

    public class FuelRecordService : IFuelRecordService
    {
        private readonly ApplicationDbContext _context;

        public FuelRecordService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<FuelRecord>> GetAllFuelRecordsAsync()
        {
            return await _context.FuelRecords
                .Where(f => !f.IsDeleted)
                .Include(f => f.Vehicle) // Inclui dados do veículo
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<FuelRecord?> GetFuelRecordByIdAsync(int id)
        {
            return await _context.FuelRecords
                .Where(f => f.Id == id && !f.IsDeleted)
                .Include(f => f.Vehicle)
                .FirstOrDefaultAsync();
        }

        public async Task<List<FuelRecord>> GetFuelRecordsByVehicleAsync(int vehicleId)
        {
            return await _context.FuelRecords
                .Where(f => f.VehicleId == vehicleId && !f.IsDeleted)
                .Include(f => f.Vehicle)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Vehicles
                .Where(v => !v.IsDeleted)
                .OrderBy(v => v.Name)
                .ToListAsync();
        }

        public async Task<FuelRecord> CreateFuelRecordAsync(FuelRecord fuelRecord)
        {
            fuelRecord.IsDeleted = false;
            fuelRecord.CreatedAt = DateTime.UtcNow;
            
            // Calcula o custo total se não foi fornecido
            if (fuelRecord.TotalCost == 0)
            {
                fuelRecord.TotalCost = fuelRecord.FuelAmount * fuelRecord.FuelPricePerUnit;
            }
            
            _context.FuelRecords.Add(fuelRecord);
            await _context.SaveChangesAsync();
            return fuelRecord;
        }

        public async Task<FuelRecord> UpdateFuelRecordAsync(FuelRecord fuelRecord)
        {
            fuelRecord.IsDeleted = false; // Garante que não está deletado
            
            // Recalcula o custo total se os valores foram alterados
            fuelRecord.TotalCost = fuelRecord.FuelAmount * fuelRecord.FuelPricePerUnit;
            
            _context.FuelRecords.Update(fuelRecord);
            await _context.SaveChangesAsync();
            return fuelRecord;
        }

        public async Task<bool> DeleteFuelRecordAsync(int id)
        {
            var fuelRecord = await _context.FuelRecords.FindAsync(id);
            if (fuelRecord == null)
                return false;

            // Exclusão lógica (soft delete)
            fuelRecord.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

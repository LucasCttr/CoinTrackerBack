using MongoDB.Driver;
using coninTracker.API.Data;
using coninTracker.API.Models;
using coninTracker.API.DTOs.Auth;

namespace coninTracker.API.Services;

public class AuthService : IAuthService
{
    private readonly MongoDbContext _context;
    private readonly IJwtService _jwtService;
    
    public AuthService(MongoDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }
    
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // 1. Verificar si email ya existe
        var existingUser = await _context.Users
            .Find(u => u.Email == registerDto.Email)
            .FirstOrDefaultAsync();
            
        if (existingUser != null)
            throw new Exception("Email ya está registrado");
        
        // 2. Hash de la contraseña
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
        
        // 3. Separar el nombre completo
        var nameParts = registerDto.Name?.Split(' ', 2) ?? ["", ""];
        var name = nameParts.Length > 0 ? nameParts[0] : "";
        
        // 4. Crear usuario
        var user = new User
        {
            Email = registerDto.Email,
            Password = hashedPassword,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };
        
        await _context.Users.InsertOneAsync(user);
        
        // 4. Generar token
        var token = _jwtService.GenerateToken(user);
        
        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Name = user.Name,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
    
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // 1. Buscar usuario
        var user = await _context.Users
            .Find(u => u.Email == loginDto.Email && u.IsActive)
            .FirstOrDefaultAsync();
            
        if (user == null)
            throw new Exception("Credenciales inválidas");
        
        // 2. Verificar contraseña
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            throw new Exception("Credenciales inválidas");
        
        // 3. Generar token
        var token = _jwtService.GenerateToken(user);
        
        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Name = user.Name,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}
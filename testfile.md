```c++
using Microsoft.AspNetCore.Mvc;
using UniPal_API.Database;
using UniPal_API.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UniPalDbContext _context;
    private readonly ILogger<AuthenticationController> _logger;
    
    public AuthenticationController(UniPalDbContext context, ILogger<AuthenticationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User login)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == login.Id);
            
            if (user == null || user.PasswordHash != login.PasswordHash)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            
            return Ok(new { message = "Login successful" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return StatusCode(500, new { message = "Server error" });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User newUser)
    {
        try
        {
            _logger.LogInformation($"Registration attempt for email: {newUser.Id}");

            // Validate email domain
            if (!newUser.Id.ToLower().EndsWith("@hull.ac.uk"))
            {
                return BadRequest(new { message = "Only @hull.ac.uk email addresses are allowed to register." });
            }

            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
            if (existingUser != null)
            {
                return BadRequest(new { message = "An account with this email already exists." });
            }

            // Validate password (basic validation)
            if (string.IsNullOrEmpty(newUser.PasswordHash) || newUser.PasswordHash.Length < 6)
            {
                return BadRequest(new { message = "Password must be at least 6 characters long." });
            }

            // In production, you should hash the password here!
            // newUser.PasswordHash = BCrypt.HashPassword(newUser.PasswordHash);

            // Add the new user
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User registered successfully: {newUser.Id}");
            
            return Ok(new { message = "Registration successful! You can now login." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration error");
            return StatusCode(500, new { message = "An error occurred during registration." });
        }
    }

    // Optional: Add an endpoint to check email availability
    [HttpGet("check-email/{email}")]
    public async Task<IActionResult> CheckEmailAvailability(string email)
    {
        var exists = await _context.Users.AnyAsync(u => u.Id == email);
        return Ok(new { available = !exists });
    }
}

```

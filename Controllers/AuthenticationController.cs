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
            _logger.LogInformation($"========== REGISTRATION ATTEMPT ==========");
            _logger.LogInformation($"Email: {newUser.Id}");
            _logger.LogInformation($"Password Hash Length: {newUser.PasswordHash?.Length ?? 0}");

            // Validate input
            if (newUser == null || string.IsNullOrEmpty(newUser.Id) || string.IsNullOrEmpty(newUser.PasswordHash))
            {
                _logger.LogWarning("Registration failed: Missing required fields");
                return BadRequest(new { message = "Email and password are required." });
            }

            // Validate email domain
            if (!newUser.Id.ToLower().EndsWith("@hull.ac.uk"))
            {
                _logger.LogWarning($"Registration failed: Invalid email domain - {newUser.Id}");
                return BadRequest(new { message = "Only @hull.ac.uk email addresses are allowed to register." });
            }

            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
            if (existingUser != null)
            {
                _logger.LogWarning($"Registration failed: Email already exists - {newUser.Id}");
                return BadRequest(new { message = "An account with this email already exists." });
            }

            // Validate password
            if (string.IsNullOrEmpty(newUser.PasswordHash) || newUser.PasswordHash.Length < 6)
            {
                _logger.LogWarning($"Registration failed: Password too short - {newUser.PasswordHash?.Length ?? 0} characters");
                return BadRequest(new { message = "Password must be at least 6 characters long." });
            }

            // Add registered date if your model has it
            // newUser.RegisteredDate = DateTime.Now;

            // Add the new user
            await _context.Users.AddAsync(newUser);
            var saveResult = await _context.SaveChangesAsync();

            _logger.LogInformation($"Database save result: {saveResult} rows affected");
            _logger.LogInformation($"✅ User registered successfully: {newUser.Id}");
            _logger.LogInformation($"========== REGISTRATION SUCCESS ==========");

            return Ok(new { message = "Registration successful! You can now login." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "========== REGISTRATION ERROR ==========");
            _logger.LogError($"Error: {ex.Message}");
            _logger.LogError($"Stack Trace: {ex.StackTrace}");
            return StatusCode(500, new { message = $"Server error: {ex.Message}" });
        }
    }
}










/*
using Microsoft.AspNetCore.Mvc;
using UniPal_API.Database;

using UniPal_API.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UniPalDbContext _context;
    public AuthenticationController(UniPalDbContext context)
    {
        _context = context;
    }
    /*
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
        {
            return BadRequest("Email already in use.");
        }
        // Hash the password before saving (for simplicity, using plain text here)
        user.PasswordHash = user.PasswordHash; // Replace with actual hashing
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return Ok("User registered successfully.");
    }
    */
/*
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == login.Id);
        if (user == null || user.PasswordHash != login.PasswordHash) // Replace with actual password verification
        {
            return Unauthorized("Invalid email or password.");
        }
        

        return Ok(new { Message = "Login succesful" });
    }

    [HttpGet("test-user")]
    public async Task<IActionResult> TestUser()
    {
        var user = await _context.Users.ToListAsync();
       
        return Ok(user);
    }
}

*/
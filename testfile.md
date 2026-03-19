```c++
@page "/register"
@using System.Net.Http
@using System.Net.Http.Json
@inject HttpClient Http
@inject NavigationManager Navigation

<div class="register-container">
    <div class="register-box">
        <h2 class="register-title">Create UniPal Account</h2>
        
        <div class="alert alert-info">
            <p>📧 Only <strong>@@hull.ac.uk</strong> email addresses are allowed to register.</p>
        </div>

        <!-- Debug Info - Shows why button is disabled -->
        @if (!CanRegister())
        {
            <div class="debug-info" style="background: #f0f0f0; padding: 10px; border-radius: 5px; margin-bottom: 20px; font-size: 12px;">
                <strong>Debug - Why can't I register?</strong>
                <ul style="margin-top: 5px; list-style-type: none; padding-left: 0;">
                    <li style="color: @(string.IsNullOrEmpty(fullName) ? "red" : "green")">
                        @(string.IsNullOrEmpty(fullName) ? "❌" : "✅") Full name: @(string.IsNullOrEmpty(fullName) ? "Missing" : fullName)
                    </li>
                    <li style="color: @(!IsValidHullEmail(email) ? "red" : "green")">
                        @(!IsValidHullEmail(email) ? "❌" : "✅") Email: @(string.IsNullOrEmpty(email) ? "Missing" : email) 
                        @(!IsValidHullEmail(email) && !string.IsNullOrEmpty(email) ? "(must end with @@hull.ac.uk)" : "")
                    </li>
                    <li style="color: @(!IsValidPassword(password) ? "red" : "green")">
                        @(!IsValidPassword(password) ? "❌" : "✅") Password: @(!IsValidPassword(password) ? "Must be at least 6 characters" : "Valid")
                    </li>
                    <li style="color: @(password != confirmPassword ? "red" : "green")">
                        @(password != confirmPassword ? "❌" : "✅") Confirm Password: @(password != confirmPassword ? "Passwords don't match" : "Matches")
                    </li>
                </ul>
            </div>
        }

        <div class="form-group">
            <label for="fullName">Full Name:</label>
            <input type="text" id="fullName" class="form-control" @bind="fullName" @bind:event="oninput" placeholder="Enter your full name" />
        </div>

        <div class="form-group">
            <label for="email">Email:</label>
            <input type="email" id="email" class="form-control" @bind="email" @bind:event="oninput" placeholder="your.name@@hull.ac.uk" />
            @if (!string.IsNullOrEmpty(email) && !IsValidHullEmail(email))
            {
                <small class="text-danger">❌ Only @@hull.ac.uk email addresses are allowed</small>
            }
            else if (!string.IsNullOrEmpty(email) && IsValidHullEmail(email))
            {
                <small class="text-success">✅ Valid Hull email</small>
            }
        </div>

        <div class="form-group">
            <label for="password">Password:</label>
            <input type="password" id="password" class="form-control" @bind="password" @bind:event="oninput" placeholder="Create a password" />
            @if (!string.IsNullOrEmpty(password))
            {
                if (password.Length < 6)
                {
                    <small class="text-danger">❌ Password must be at least 6 characters (currently @password.Length)</small>
                }
                else
                {
                    <small class="text-success">✅ Password length OK</small>
                }
            }
            else
            {
                <small class="text-muted">Password must be at least 6 characters</small>
            }
        </div>

        <div class="form-group">
            <label for="confirmPassword">Confirm Password:</label>
            <input type="password" id="confirmPassword" class="form-control" @bind="confirmPassword" @bind:event="oninput" placeholder="Confirm your password" />
            @if (!string.IsNullOrEmpty(confirmPassword))
            {
                if (password != confirmPassword)
                {
                    <small class="text-danger">❌ Passwords do not match</small>
                }
                else
                {
                    <small class="text-success">✅ Passwords match</small>
                }
            }
        </div>

        <button class="btn-register" @onclick="HandleRegister" disabled="@(!CanRegister())">
            @(isLoading ? "Creating Account..." : "Register")
        </button>

        @if (!string.IsNullOrEmpty(errorMessage))
        {
            <div class="error-message" style="color: red; margin-top: 15px; padding: 10px; background-color: #ffebee; border-radius: 5px;">
                @errorMessage
            </div>
        }

        @if (!string.IsNullOrEmpty(successMessage))
        {
            <div class="success-message" style="color: green; margin-top: 15px; padding: 10px; background-color: #e8f5e8; border-radius: 5px;">
                @successMessage
            </div>
        }

        <div class="login-link" style="margin-top: 20px; text-align: center;">
            Already have an account? <a href="/">Login here</a>
        </div>
    </div>
</div>

@code {
    private string fullName = string.Empty;
    private string email = string.Empty;
    private string password = string.Empty;
    private string confirmPassword = string.Empty;
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;
    private bool isLoading = false;

    private bool IsValidHullEmail(string email)
    {
        return !string.IsNullOrEmpty(email) && email.Trim().ToLower().EndsWith("@hull.ac.uk");
    }

    private bool IsValidPassword(string password)
    {
        return !string.IsNullOrEmpty(password) && password.Length >= 6;
    }

    private bool CanRegister()
    {
        return !string.IsNullOrEmpty(fullName) &&
               IsValidHullEmail(email) &&
               IsValidPassword(password) &&
               password == confirmPassword;
    }

    private async Task HandleRegister()
    {
        isLoading = true;
        errorMessage = string.Empty;
        successMessage = string.Empty;
        
        // Force the UI to update immediately
        StateHasChanged();

        try
        {
            Console.WriteLine("=== REGISTRATION ATTEMPT ===");
            Console.WriteLine($"Full Name: {fullName}");
            Console.WriteLine($"Email: {email}");
            Console.WriteLine($"Password length: {password?.Length ?? 0}");
            
            var newUser = new
            {
                id = email.Trim(),
                passwordHash = password.Trim()
            };
            
            Console.WriteLine("Sending request to API...");
            Console.WriteLine($"Request URL: api/Authentication/register");
            Console.WriteLine($"Request Data: {{ id: {newUser.id}, passwordHash: *** }}");

            var response = await Http.PostAsJsonAsync("api/Authentication/register", newUser);
            
            Console.WriteLine($"Response Status Code: {(int)response.StatusCode} {response.StatusCode}");
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("✅ Registration successful!");
                successMessage = "Account created successfully! Redirecting to login...";
                
                // Clear form
                fullName = "";
                email = "";
                password = "";
                confirmPassword = "";
                
                // Force UI update
                StateHasChanged();
                
                await Task.Delay(3000);
                Navigation.NavigateTo("/");
            }
            else
            {
                Console.WriteLine($"❌ Registration failed: {responseContent}");
                
                // Try to parse the error message
                try
                {
                    var errorObj = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);
                    if (errorObj != null && errorObj.ContainsKey("message"))
                    {
                        errorMessage = errorObj["message"];
                    }
                    else
                    {
                        errorMessage = responseContent;
                    }
                }
                catch
                {
                    errorMessage = responseContent ?? $"Registration failed with status {(int)response.StatusCode}";
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"🌐 Network Error: {ex.Message}");
            errorMessage = "Cannot connect to server. Make sure the API is running.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 Exception: {ex.GetType().Name} - {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            errorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
            Console.WriteLine("=== REGISTRATION ATTEMPT END ===");
        }
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













```

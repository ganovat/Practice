```c++
@page "/register"
@using System.Net.Http
@using System.Net.Http.Json
@inject HttpClient Http
@inject NavigationManager Navigation

<div class="register-container">
    <div class="register-box">
        <h2 class="register-title">Create UniPal Account</h2>
        
        <div class="alert alert-info" style="background-color: #e3f2fd; padding: 10px; border-radius: 5px; margin-bottom: 20px;">
            <p style="margin: 0; color: #0d47a1;">📧 Only <strong>@hull.ac.uk</strong> email addresses are allowed to register.</p>
        </div>

        <div class="form-group">
            <label for="fullName">Full Name:</label>
            <input type="text" id="fullName" class="form-control" @bind="fullName" placeholder="Enter your full name" />
        </div>

        <div class="form-group">
            <label for="email">Email:</label>
            <input type="email" id="email" class="form-control" @bind="email" placeholder="your.name@hull.ac.uk" />
            @if (!string.IsNullOrEmpty(email) && !IsValidHullEmail(email))
            {
                <small class="text-danger">Only @hull.ac.uk email addresses are allowed</small>
            }
        </div>

        <div class="form-group">
            <label for="password">Password:</label>
            <input type="password" id="password" class="form-control" @bind="password" placeholder="Create a password" />
            <small class="text-muted">Password must be at least 6 characters</small>
        </div>

        <div class="form-group">
            <label for="confirmPassword">Confirm Password:</label>
            <input type="password" id="confirmPassword" class="form-control" @bind="confirmPassword" placeholder="Confirm your password" />
            @if (password != confirmPassword && confirmPassword != "")
            {
                <small class="text-danger">Passwords do not match</small>
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

        try
        {
            // Create user object matching your User model
            var newUser = new
            {
                id = email.Trim(),           // Email as ID
                passwordHash = password.Trim()  // In production, hash this!
                // Note: You might want to add Name/FullName to your User model
            };

            // Call your registration endpoint
            var response = await Http.PostAsJsonAsync("api/Authentication/register", newUser);

            if (response.IsSuccessStatusCode)
            {
                successMessage = "Account created successfully! Redirecting to login...";
                
                // Clear form
                fullName = "";
                email = "";
                password = "";
                confirmPassword = "";
                
                // Redirect to login after 3 seconds
                await Task.Delay(3000);
                Navigation.NavigateTo("/");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                errorMessage = error ?? "Registration failed. Please try again.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}

```

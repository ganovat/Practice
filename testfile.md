```c++
@page "/profile"
@using System.Net.Http
@using System.Net.Http.Json
@inject HttpClient Http
@inject NavigationManager Navigation

<div class="profile-container">
    <!-- Cover Photo Section -->
    <div class="cover-photo">
        <!-- Optional cover photo -->
    </div>

    <!-- Profile Header -->
    <div class="profile-header">
        <div class="profile-avatar">
            @if (!string.IsNullOrEmpty(profilePicture))
            {
                <img src="@profilePicture" alt="Profile Picture" class="avatar-img" />
            }
            else
            {
                <div class="avatar-placeholder">
                    <span>@initials</span>
                </div>
            }
            
            <!-- Edit button for profile picture -->
            <button class="edit-avatar-btn" @onclick="OpenImageUpload">
                <i class="fas fa-camera"></i>
            </button>
        </div>
        
        <div class="profile-info">
            <h2 class="profile-name">@name</h2>
            <p class="profile-email">@email</p>
            <p class="profile-joined">Joined: @joinedDate</p>
        </div>
        
        <button class="edit-profile-btn" @onclick="ToggleEditMode">
            @(isEditing ? "Cancel" : "Edit Profile")
        </button>
    </div>

    @if (isEditing)
    {
        <!-- Edit Mode -->
        <div class="edit-section">
            <h3>Edit Profile</h3>
            
            <div class="form-group">
                <label for="editName">Name:</label>
                <input type="text" id="editName" class="form-control" @bind="editName" />
            </div>
            
            <div class="form-group">
                <label for="editBio">Bio:</label>
                <textarea id="editBio" class="form-control" @bind="editBio" rows="4"></textarea>
            </div>
            
            <div class="form-group">
                <label for="editLocation">Location:</label>
                <input type="text" id="editLocation" class="form-control" @bind="editLocation" />
            </div>
            
            <div class="form-group">
                <label for="editInterests">Interests (comma separated):</label>
                <input type="text" id="editInterests" class="form-control" @bind="editInterests" />
            </div>
            
            <div class="button-group">
                <button class="btn btn-primary" @onclick="SaveProfile">Save Changes</button>
                <button class="btn btn-secondary" @onclick="ToggleEditMode">Cancel</button>
            </div>
        </div>
    }
    else
    {
        <!-- View Mode -->
        <div class="profile-content">
            <!-- Bio Section -->
            <div class="bio-section">
                <h3>About Me</h3>
                <p class="bio-text">@(string.IsNullOrEmpty(bio) ? "No bio added yet." : bio)</p>
            </div>

            <!-- Details Grid -->
            <div class="details-section">
                <h3>Details</h3>
                <div class="details-grid">
                    <div class="detail-item">
                        <span class="detail-label">📍 Location:</span>
                        <span class="detail-value">@(string.IsNullOrEmpty(location) ? "Not specified" : location)</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">🎓 University:</span>
                        <span class="detail-value">@university</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">📚 Course:</span>
                        <span class="detail-value">@course</span>
                    </div>
                    <div class="detail-item">
                        <span class="detail-label">🎂 Age:</span>
                        <span class="detail-value">@age</span>
                    </div>
                </div>
            </div>

            <!-- Interests Section -->
            <div class="interests-section">
                <h3>Interests</h3>
                @if (interestsList.Any())
                {
                    <div class="interests-list">
                        @foreach (var interest in interestsList)
                        {
                            <span class="interest-tag">@interest</span>
                        }
                    </div>
                }
                else
                {
                    <p class="no-interests">No interests added yet.</p>
                }
            </div>

            <!-- Stats Section -->
            <div class="stats-section">
                <div class="stat-card">
                    <span class="stat-number">@postsCount</span>
                    <span class="stat-label">Posts</span>
                </div>
                <div class="stat-card">
                    <span class="stat-number">@friendsCount</span>
                    <span class="stat-label">Friends</span>
                </div>
                <div class="stat-card">
                    <span class="stat-number">@eventsCount</span>
                    <span class="stat-label">Events</span>
                </div>
            </div>
        </div>
    }
</div>

<!-- Hidden file input for image upload -->
<input type="file" id="imageUpload" accept="image/*" style="display: none;" @onchange="OnImageSelected" />

@code {
    private bool isEditing = false;
    private string name = "John Doe";
    private string email = "john.doe@university.edu";
    private string joinedDate = "March 2026";
    private string profilePicture = "";
    private string bio = "Computer Science student passionate about coding and building cool stuff! 🚀";
    private string location = "London, UK";
    private string university = "University of Hull";
    private string course = "Computer Science";
    private string age = "21";
    private string interests = "Coding, Gaming, Football, Music";
    private string initials = "JD";
    private int postsCount = 15;
    private int friendsCount = 42;
    private int eventsCount = 8;

    // Edit mode fields
    private string editName;
    private string editBio;
    private string editLocation;
    private string editInterests;

    // Computed property for interests list
    private List<string> interestsList => interests.Split(',').Select(i => i.Trim()).ToList();

    protected override void OnInitialized()
    {
        // Load user data from your API or local storage
        LoadUserData();
        CalculateInitials();
    }

    private void LoadUserData()
    {
        // TODO: Load actual user data from your API or local storage
        // This would typically come from a service or API call
        editName = name;
        editBio = bio;
        editLocation = location;
        editInterests = interests;
    }

    private void CalculateInitials()
    {
        if (!string.IsNullOrEmpty(name))
        {
            var names = name.Split(' ');
            if (names.Length >= 2)
            {
                initials = $"{names[0][0]}{names[1][0]}";
            }
            else if (names.Length == 1)
            {
                initials = names[0][0].ToString();
            }
        }
    }

    private void ToggleEditMode()
    {
        isEditing = !isEditing;
        if (isEditing)
        {
            // Load current values into edit fields
            editName = name;
            editBio = bio;
            editLocation = location;
            editInterests = interests;
        }
    }

    private async Task SaveProfile()
    {
        // Update profile with edited values
        name = editName;
        bio = editBio;
        location = editLocation;
        interests = editInterests;
        
        CalculateInitials();
        
        // TODO: Save to your API
        // await Http.PostAsJsonAsync("api/User/update", updatedUserData);
        
        isEditing = false;
    }

    private void OpenImageUpload()
    {
        // Trigger file input click
        var imageUpload = System.Diagnostics.Debugger.IsAttached 
            ? null // For debugging
            : null; // You'll need to use JavaScript interop for this
        // Better approach - use JavaScript interop
        // await JSRuntime.InvokeVoidAsync("document.getElementById('imageUpload').click");
    }

    private async Task OnImageSelected(ChangeEventArgs e)
    {
        // TODO: Handle image upload
        // This would typically use JavaScript interop to read the file
        // Then upload to your API or cloud storage
    }
}


```

using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KAssistant.Diagnostics
{
    /// <summary>
    /// Diagnostic tool to test Kavita API endpoints directly
    /// Run this to see what the actual API returns
    /// </summary>
    public class KavitaDiagnostics
    {
        public static async Task RunDiagnostics(string baseUrl, string username, string password)
        {
            Console.WriteLine("=== KAVITA API DIAGNOSTICS ===\n");
            
            using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Step 1: Test connectivity
                Console.WriteLine("1. Testing connectivity to Kavita server...");
                var response = await client.GetAsync("/api/Server/server-info");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ? Connected! Response: {content.Substring(0, Math.Min(200, content.Length))}...");
                }
                else
                {
                    Console.WriteLine($"   ? Connection failed: {response.StatusCode}");
                    return;
                }

                // Step 2: Test login
                Console.WriteLine("\n2. Testing login...");
                var loginRequest = new
                {
                    username = username,
                    password = password
                };
                var loginJson = JsonSerializer.Serialize(loginRequest);
                var loginContent = new StringContent(loginJson, Encoding.UTF8, "application/json");
                
                response = await client.PostAsync("/api/Account/login", loginContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ? Login successful!");
                    
                    // Extract token
                    var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("token", out var tokenElement))
                    {
                        var token = tokenElement.GetString();
                        client.DefaultRequestHeaders.Authorization = 
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        var tokenDisplay = token != null && token.Length > 50 ? token.Substring(0, 50) : token;
                        Console.WriteLine($"   Token: {tokenDisplay}...");
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ? Login failed: {response.StatusCode}");
                    Console.WriteLine($"   Response: {content}");
                    return;
                }

                // Step 3: Test libraries endpoint
                Console.WriteLine("\n3. Testing libraries endpoint...");
                response = await client.GetAsync("/api/Library/libraries");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ? Libraries endpoint works!");
                    Console.WriteLine($"   Response: {content.Substring(0, Math.Min(300, content.Length))}...");
                }
                else
                {
                    Console.WriteLine($"   ? Libraries failed: {response.StatusCode}");
                }

                // Step 4: Test recently-added endpoint
                Console.WriteLine("\n4. Testing recently-added endpoint...");
                var recentlyAddedRequest = new
                {
                    pageNumber = 0,
                    pageSize = 5
                };
                var recentlyAddedJson = JsonSerializer.Serialize(recentlyAddedRequest);
                var recentlyAddedContent = new StringContent(recentlyAddedJson, Encoding.UTF8, "application/json");
                
                response = await client.PostAsync("/api/Series/recently-added-v2", recentlyAddedContent);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ? Recently-added endpoint works!");
                    Console.WriteLine($"   Response (first 500 chars): {content.Substring(0, Math.Min(500, content.Length))}...");
                    
                    // Try to parse
                    try
                    {
                        var doc = JsonDocument.Parse(content);
                        if (doc.RootElement.TryGetProperty("result", out var resultElement))
                        {
                            Console.WriteLine($"   Series count in response: {resultElement.GetArrayLength()}");
                            
                            if (resultElement.GetArrayLength() > 0)
                            {
                                Console.WriteLine("\n   First series object structure:");
                                var firstSeries = resultElement[0];
                                foreach (var property in firstSeries.EnumerateObject())
                                {
                                    Console.WriteLine($"      - {property.Name}: {property.Value.ValueKind}");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ? Could not parse response: {ex.Message}");
                    }
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ? Recently-added failed: {response.StatusCode}");
                    Console.WriteLine($"   Response: {content}");
                }

                // Step 5: Test all-v2 endpoint
                Console.WriteLine("\n5. Testing all-v2 endpoint...");
                var allV2Request = new
                {
                    libraryIds = new[] { 1 },
                    pageNumber = 0,
                    pageSize = 5
                };
                var allV2Json = JsonSerializer.Serialize(allV2Request);
                var allV2Content = new StringContent(allV2Json, Encoding.UTF8, "application/json");
                
                response = await client.PostAsync("/api/Series/all-v2", allV2Content);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ? All-v2 endpoint works!");
                    Console.WriteLine($"   Response (first 500 chars): {content.Substring(0, Math.Min(500, content.Length))}...");
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"   ? All-v2 failed: {response.StatusCode}");
                    Console.WriteLine($"   Response: {content}");
                }

                Console.WriteLine("\n=== DIAGNOSTICS COMPLETE ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n? Exception occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}

using System;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using Kontent.Ai.Management.Tests.Base;
using Kontent.Ai.Management.Extensions;
using Kontent.Ai.Management.Models.Shared;
using Kontent.Ai.Management.Models.Languages;

namespace Kontent.Ai.Management.Tests.ManagementClientTests;

public class LanguageTests : IClassFixture<FileSystemFixture>
{
    private readonly FileSystemFixture _fileSystemFixture;

    public LanguageTests(FileSystemFixture fileSystemFixture)
    {
        _fileSystemFixture = fileSystemFixture;
        _fileSystemFixture.SetSubFolder("Language");
    }

    [Fact]
    public async void CreateLanguage_CreatesLanguage()
    {
        var client = _fileSystemFixture.CreateMockClientWithResponse("CreateLanguage_CreatesLanguage.json");

        var newLanguage = new LanguageCreateModel
        {
            Name = "German (Germany)",
            Codename = "de-DE",
            IsActive = false,
            ExternalId = "standard-german",
            FallbackLanguage = Reference.ById(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        };

        var response = await client.CreateLanguageAsync(newLanguage);

        using (new AssertionScope())
        {
            response.Name.Should().BeEquivalentTo(newLanguage.Name);
            response.Codename.Should().BeEquivalentTo(newLanguage.Codename);
            response.ExternalId.Should().BeEquivalentTo(newLanguage.ExternalId);
            response.FallbackLanguage.Id.Should().Be(newLanguage.FallbackLanguage.Id);
        }
    }

    [Fact]
    public async void ListLanguages_ListsLanguages()
    {
        var client = _fileSystemFixture.CreateMockClientWithResponse("LanguagesPage1.json", "LanguagesPage2.json", "LanguagesPage3.json");

        var expectedItems = _fileSystemFixture.GetItemsOfExpectedListingResponse<LanguageModel>("LanguagesPage1.json", "LanguagesPage2.json", "LanguagesPage3.json");

        var response = await client.ListLanguagesAsync().GetAllAsync();

        response.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public async void GetLanguage_ById_GetsLanguage()
    {
        var client = _fileSystemFixture.CreateMockClientWithResponse("SingleLanguageResponse.json");

        var response = await client.GetLanguageAsync(Reference.ById(Guid.Parse("00000000-0000-0000-0000-000000000000")));

        using (new AssertionScope())
        {
            response.Name.Should().BeEquivalentTo("Default project language");
            response.Codename.Should().BeEquivalentTo("default");
            response.ExternalId.Should().BeEquivalentTo("string");
            response.FallbackLanguage.Id.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000000"));
            response.IsActive.Should().BeTrue();
            response.IsDefault.Should().BeTrue();
        }
    }

    [Fact]
    public async void GetLanguage_ByCodename_GetsLanguage()
    {
        var client = _fileSystemFixture.CreateMockClientWithResponse("SingleLanguageResponse.json");

        var response = await client.GetLanguageAsync(Reference.ByCodename("default"));

        using (new AssertionScope())
        {
            response.Name.Should().BeEquivalentTo("Default project language");
            response.Codename.Should().BeEquivalentTo("default");
            response.ExternalId.Should().BeEquivalentTo("string");
            response.FallbackLanguage.Id.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000000"));
            response.IsActive.Should().BeTrue();
            response.IsDefault.Should().BeTrue();
        }
    }

    [Fact]
    public async void GetLanguage_ByExternalId_GetsLanguage()
    {
        var client = _fileSystemFixture.CreateMockClientWithResponse("SingleLanguageResponse.json");

        var response = await client.GetLanguageAsync(Reference.ByExternalId("string"));

        using (new AssertionScope())
        {
            response.Name.Should().BeEquivalentTo("Default project language");
            response.Codename.Should().BeEquivalentTo("default");
            response.ExternalId.Should().BeEquivalentTo("string");
            response.FallbackLanguage.Id.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000000"));
            response.IsActive.Should().BeTrue();
            response.IsDefault.Should().BeTrue();
        }
    }

    [Fact]
    public async void ModifyLanguages_Replace_ModifiesLanguages()
    {
        var client = _fileSystemFixture.CreateMockClientWithResponse("ModifyLanguages_Replace_ModifiesLanguages.json");

        var patchModel = new[]
        {
            new LanguagePatchModel
            {
                PropertyName = LanguagePropertyName.FallbackLanguage,
                Value = new {
                    Codename = "en-US"
                }
            },
            new LanguagePatchModel
            {
                PropertyName = LanguagePropertyName.Name,
                Value = "Deutsch"
            }
        };

        var modifiedLanguage = await client.ModifyLanguageAsync(Reference.ByCodename("de-DE"), patchModel);
        using (new AssertionScope())
        {
            modifiedLanguage.Name.Should().BeEquivalentTo("Deutsch");
            modifiedLanguage.FallbackLanguage.Id.Should().Be(Guid.Parse("00000000-0000-0000-0000-000000000000"));

        }
    }
}

using RestApia.Extensions.Import.Postman;
namespace RestApia.Experiments.Tests.Extensions.Import;

public class PostmanImportExperiments
{
    [TestCase("Postman_Echo.postman_collection.json")]
    [TestCase("RestApia_Test.postman_collection.json")]
    public void Import_FromFile_NotEmpty(string fileName)
    {
        // arrange
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "_assets", "auth-postman", fileName);
        File.Exists(filePath).Should().BeTrue();

        var service = new PostmanImportService();

        // act
        var result = service.Import(filePath);

        // assert
        result.Should().NotBeNull();
    }
}

using Xunit;

namespace BasicTools.Client.Pages
{

    public class JsonYamlTests
    {
        [Theory]
        [InlineData("1")]
        [InlineData("{")]
        [InlineData(@"{""test""""value""}")]
        public void ProcessErrorsAreHandled(string input)
        {
            //arrange
            var sut = new JsonYaml
            {
                Input = input
            };

            //act
            sut.Process();

            //assert
            Assert.True(sut.Output.IsError);
        }    
    }
}

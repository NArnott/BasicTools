using Xunit;

namespace BasicTools.Client.Pages
{

    public class JsonTests
    {
        [Theory]
        [InlineData("1")]
        [InlineData("{")]
        [InlineData(@"{""test""""value""}")]
        public void ProcessErrorsAreHandled(string input)
        {
            //arrange
            var sut = new Json
            {
                Input = input
            };

            //act
            sut.Process();

            //assert
            Assert.True(sut.OutputIsError);
        }    
    }
}

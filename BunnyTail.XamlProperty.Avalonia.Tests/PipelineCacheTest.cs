namespace BunnyTail.XamlProperty;

using SourceGenerateHelper.Testing;

public sealed class PipelineCacheTest
{
    private const string Source =
        """
        using BunnyTail.XamlProperty;
        using Avalonia;

        namespace Test;

        public partial class TestElement : AvaloniaObject
        {
            [StyledProperty]
            public partial string? Text { get; set; }
        }
        """;

    private const string UnrelatedSource =
        """
        namespace Other;

        internal sealed class Unrelated;
        """;

    private const string AddedTargetSource =
        """
        using BunnyTail.XamlProperty;
        using Avalonia;

        namespace Test;

        public partial class AddedElement : AvaloniaObject
        {
            [StyledProperty]
            public partial string? Text { get; set; }
        }
        """;

    private const string BaseCallbackSource =
        """
        using BunnyTail.XamlProperty;
        using Avalonia;

        namespace Test;

        public class BaseElement : AvaloniaObject
        {
            protected double CoerceScale(double value) => value;
        }

        public partial class DerivedElement : BaseElement
        {
            [StyledProperty(Coerce = nameof(CoerceScale))]
            public partial double Scale { get; set; }
        }
        """;

    // ------------------------------------------------------------
    // Cache
    // ------------------------------------------------------------

    [Fact]
    public void UnrelatedEditKeepsModelCached()
    {
        // Arrange & Act
        var result = GeneratorTestHelper.RunIncremental(Source, UnrelatedSource);

        // Assert
        Assert.Equal(result.FirstGeneratedText, result.SecondGeneratedText);
        Assert.NotEmpty(result.OutputReasons);
        Assert.DoesNotContain(result.OutputReasons, static x => x.IsChanged());
    }

    [Fact]
    public void TargetEditRebuildsModel()
    {
        // Arrange & Act
        var result = GeneratorTestHelper.RunIncremental(Source, AddedTargetSource);

        // Assert
        Assert.Contains(result.OutputReasons, static x => x.IsChanged());
    }

    [Fact]
    public void UnrelatedEditKeepsBaseCallbackModelCached()
    {
        // Arrange & Act
        var result = GeneratorTestHelper.RunIncremental(BaseCallbackSource, UnrelatedSource);

        // Assert
        Assert.Equal(result.FirstGeneratedText, result.SecondGeneratedText);
        Assert.NotEmpty(result.OutputReasons);
        Assert.DoesNotContain(result.OutputReasons, static x => x.IsChanged());
    }
}

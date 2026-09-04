using ArenaSystemsLab.Editor;
using NUnit.Framework;

namespace ArenaSystemsLab.Tests.EditMode
{
    public sealed class ArenaProjectValidatorTests
    {
        [Test]
        public void ValidateCurrentProject_ReturnsNoErrors()
        {
            Assert.That(ArenaProjectValidator.ValidateCurrentProject(), Is.Empty);
        }

        [Test]
        public void ValidateSnapshot_WithBrokenConfiguration_ReportsEachProblem()
        {
            var errors = ArenaProjectValidator.ValidateSnapshot(
                "6000.5.1f1",
                "6000.5.2f1",
                false,
                true,
                false,
                false);

            Assert.That(errors, Has.Count.EqualTo(4));
            Assert.That(errors[0], Does.Contain("Editor version mismatch"));
            Assert.That(errors[1], Does.Contain("Build Settings"));
            Assert.That(errors[2], Does.Contain("Player/Move"));
            Assert.That(errors[3], Does.Contain("Player/Attack"));
        }
    }
}

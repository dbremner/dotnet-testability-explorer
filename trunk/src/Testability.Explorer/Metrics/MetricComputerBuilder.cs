using System.IO;

namespace Thinklouder.Testability.Metrics
{
    /**
     * For use in tests, this is a preferable way to construct a {@code MetricComputer},
     * as opposed to inheriting from a TestCase that does it for you. Prefer composition
     * over inheritance.
     *
     * @author Jonathan Andrew Wolter
     */
    public class MetricComputerBuilder
    {

        private IClassRepository repo = new ClrClassRepository();
        private Stream printStream = null;
        private RegExpWhiteList whitelist = new RegExpWhiteList();

        public MetricComputerBuilder withClassRepository(IClassRepository repository)
        {
            this.repo = repository;
            return this;
        }

        public MetricComputerBuilder withPrintStream(Stream stream)
        {
            this.printStream = stream;
            return this;
        }

        public MetricComputerBuilder withWhitelist(RegExpWhiteList regExpWhitelist)
        {
            this.whitelist = regExpWhitelist;
            return this;
        }

        public MetricComputer build()
        {
            return new MetricComputer(repo, printStream, whitelist, 1);
        }

    }

}
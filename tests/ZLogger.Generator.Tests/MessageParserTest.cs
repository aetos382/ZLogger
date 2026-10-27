using System.Linq;

namespace ZLogger.Generator.Tests
{
    public class MessageParserTest
    {
        [Fact]
        public void BasicCase()
        {
            {
                MessageParser.TryParseFormat(null, out var segments).ShouldBeTrue();
                MessageParser.TryParseFormat("", out segments).ShouldBeTrue();
            }

            {
                MessageParser.TryParseFormat("Could not open socket to {hostName} {ipAddress}.", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(5);
                segments[0].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[0].TextSegment.ShouldBe("Could not open socket to ");
                segments[0].TextSegment.ShouldBe(segments[0].UnescapedTextSegment);

                segments[1].Kind.ShouldBe(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.ShouldBe("hostName");
                (segments[1].Alignment, segments[1].FormatString).ShouldBe((null, null));

                segments[2].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[2].TextSegment.ShouldBe(" ");
                segments[2].TextSegment.ShouldBe(segments[2].UnescapedTextSegment);

                segments[3].Kind.ShouldBe(MessageSegmentKind.NameParameter);
                segments[3].NameParameter.ShouldBe("ipAddress");
                (segments[3].Alignment, segments[3].FormatString).ShouldBe((null, null));

                segments[4].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[4].TextSegment.ShouldBe(".");
                segments[4].TextSegment.ShouldBe(segments[4].UnescapedTextSegment);

                string.Concat(segments.Select(x => x.ToString())).ShouldBe("Could not open socket to {hostName} {ipAddress}.");
            }
        }

        [Fact]
        public void Test2()
        {
            // Begin
            {
                MessageParser.TryParseFormat("{0}aaa", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(2);
                segments[0].Kind.ShouldBe(MessageSegmentKind.IndexParameter);
                segments[0].IndexParameter.ShouldBe(0);
                segments[1].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[1].TextSegment.ShouldBe("aaa");
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("{0}aaa");
            }
            // End
            {
                MessageParser.TryParseFormat("aaa{0}", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(2);
                segments[0].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[0].TextSegment.ShouldBe("aaa");
                segments[1].Kind.ShouldBe(MessageSegmentKind.IndexParameter);
                segments[1].IndexParameter.ShouldBe(0);
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("aaa{0}");
            }
            // Escape Begin/End
            {
                MessageParser.TryParseFormat("{{aaa{foo}bbb}}", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(3);
                segments[0].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[0].TextSegment.ShouldBe("{{aaa");
                segments[0].UnescapedTextSegment.ShouldBe("{aaa");
                segments[1].Kind.ShouldBe(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.ShouldBe("foo");
                segments[2].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[2].TextSegment.ShouldBe("bbb}}");
                segments[2].UnescapedTextSegment.ShouldBe("bbb}");
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("{{aaa{foo}bbb}}");
            }
            // Escape
            {
                MessageParser.TryParseFormat("aaa{{{bar}}}bbb", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(3);
                segments[0].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[0].TextSegment.ShouldBe("aaa{{");
                segments[0].UnescapedTextSegment.ShouldBe("aaa{");
                segments[1].Kind.ShouldBe(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.ShouldBe("bar");
                segments[2].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[2].TextSegment.ShouldBe("}}bbb");
                segments[2].UnescapedTextSegment.ShouldBe("}bbb");
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("aaa{{{bar}}}bbb");
            }
            // TextOnly
            {
                MessageParser.TryParseFormat("aaabbbccc", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(1);
                segments[0].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[0].TextSegment.ShouldBe("aaabbbccc");
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("aaabbbccc");
            }
            // ParameterOnly
            {
                MessageParser.TryParseFormat("{0}", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(1);
                segments[0].Kind.ShouldBe(MessageSegmentKind.IndexParameter);
                segments[0].IndexParameter.ShouldBe(0);
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("{0}");
            }
            // ParameterOnly2
            {
                MessageParser.TryParseFormat("{0}{foo}", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(2);
                segments[0].Kind.ShouldBe(MessageSegmentKind.IndexParameter);
                segments[0].IndexParameter.ShouldBe(0);
                segments[1].Kind.ShouldBe(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.ShouldBe("foo");
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("{0}{foo}");
            }
            // AlignmentOnly
            {
                MessageParser.TryParseFormat("aaa{bbb,100}", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(2);
                segments[0].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[0].TextSegment.ShouldBe("aaa");
                segments[1].Kind.ShouldBe(MessageSegmentKind.NameParameter);
                segments[1].NameParameter.ShouldBe("bbb");
                segments[1].Alignment.ShouldBe("100");
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("aaa{bbb,100}");
            }
            // FormatOnly
            {
                MessageParser.TryParseFormat("{0:X}ccc", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(2);
                segments[0].Kind.ShouldBe(MessageSegmentKind.IndexParameter);
                segments[0].IndexParameter.ShouldBe(0);
                segments[0].FormatString.ShouldBe("X");
                segments[1].Kind.ShouldBe(MessageSegmentKind.Text);
                segments[1].TextSegment.ShouldBe("ccc");
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("{0:X}ccc");
            }
            // FullParameter
            {
                MessageParser.TryParseFormat("{a,10:X}{0,-1:Z}", out var segments).ShouldBeTrue();
                segments.Length.ShouldBe(2);
                segments[0].Kind.ShouldBe(MessageSegmentKind.NameParameter);
                segments[0].NameParameter.ShouldBe("a");
                segments[0].Alignment.ShouldBe("10");
                segments[0].FormatString.ShouldBe("X");
                segments[1].Kind.ShouldBe(MessageSegmentKind.IndexParameter);
                segments[1].IndexParameter.ShouldBe(0);
                segments[1].Alignment.ShouldBe("-1");
                segments[1].FormatString.ShouldBe("Z");
                string.Concat(segments.Select(x => x.ToString())).ShouldBe("{a,10:X}{0,-1:Z}");
            }
        }

        [Fact]
        public void ParseFailPatterns()
        {
            MessageParser.TryParseFormat("{", out var segments).ShouldBeFalse();
            MessageParser.TryParseFormat("{aaa", out segments).ShouldBeFalse();
            MessageParser.TryParseFormat("}", out segments).ShouldBeFalse();
            MessageParser.TryParseFormat("aaa}", out segments).ShouldBeFalse();
            MessageParser.TryParseFormat("a      f{           ", out segments).ShouldBeFalse();
            MessageParser.TryParseFormat("a  {}   b", out segments).ShouldBeFalse();
        }
    }
}
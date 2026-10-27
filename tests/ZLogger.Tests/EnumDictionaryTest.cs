using Shouldly;
using System;
using System.Net;
using System.Text;
using System.Text.Json;
using ZLogger.Internal;


namespace ZLogger.Tests
{

    public unsafe class EnumDictionaryTest
    {
        [Fact]
        public void Lookup()
        {
            EnumDictionary<Foo>.GetStringName(Foo.AAA).ShouldBe("AAA");
            EnumDictionary<Foo>.GetStringName(Foo.BBB).ShouldBe("BBB");
            EnumDictionary<Foo>.GetStringName(Foo.CCC).ShouldBe("CCC");
            EnumDictionary<Foo>.GetStringName((Foo)33).ShouldBeNull();

            EnumDictionary<More>.GetStringName(More.AAA).ShouldBe("AAA");
            EnumDictionary<More>.GetStringName(More.BBB).ShouldBe("BBB");
            EnumDictionary<More>.GetStringName(More.CCC).ShouldBe("CCC");
            EnumDictionary<More>.GetStringName((More)33).ShouldBeNull();

            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.A).ShouldBe("A");
            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.D).ShouldBe("D");
            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.All).ShouldBe("All");
            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.None).ShouldBe("None");
            EnumDictionary<FlagsEnum>.GetStringName(FlagsEnum.A | FlagsEnum.D).ShouldBeNull();

            EnumDictionary<Duplicate>.GetStringName(Duplicate.Foo).ShouldBe("Foo");
            (EnumDictionary<Duplicate>.GetStringName(Duplicate.Bar) is "MoreBar" or "Bar").ShouldBeTrue();
            EnumDictionary<Duplicate>.GetStringName(Duplicate.Baz).ShouldBe("Baz");
            (EnumDictionary<Duplicate>.GetStringName(Duplicate.MoreBar) is "MoreBar" or "Bar").ShouldBeTrue();
            EnumDictionary<Duplicate>.GetStringName(Duplicate.End).ShouldBe("End");
        }

        [Fact]
        public void HttpStatusCodeTest()
        {
            EnumDictionary<HttpStatusCode>.GetStringName(HttpStatusCode.InternalServerError).ShouldBe("InternalServerError");
            EnumDictionary<HttpStatusCode>.GetStringName(HttpStatusCode.RequestEntityTooLarge).ShouldBe("RequestEntityTooLarge");
            EnumDictionary<HttpStatusCode>.GetUtf8Name(HttpStatusCode.NetworkAuthenticationRequired).ToArray().ShouldBe(Encoding.UTF8.GetBytes("NetworkAuthenticationRequired"));
            EnumDictionary<HttpStatusCode>.GetJsonEncodedName(HttpStatusCode.UnprocessableEntity).ShouldBe(JsonEncodedText.Encode("UnprocessableEntity"));
        }
    }

    public enum Foo
    {
        AAA,
        BBB,
        CCC
    }

    public enum More
    {
        AAA = 10,
        BBB = 20,
        CCC = 30
    }

    [Flags]
    public enum FlagsEnum
    {
        None = 0,
        A = 1,
        B = 2,
        C = 4,
        D = 8,
        E = 16,
        All = A | B | C | D | E
    }

    public enum Duplicate
    {
        Foo = 1,
        Bar = 2,
        Baz = 3,
        MoreBar = 2,
        End = 4
    }
}

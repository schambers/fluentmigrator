using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Generators.Oracle;
using Xunit;

namespace FluentMigrator.Tests.Unit.Generators.Oracle
{
    public class OracleTypeMapTests
    {
        private OracleTypeMap _typeMap;

        public OracleTypeMapTests()
        {
            _typeMap = new OracleTypeMap();
        }

        // See https://docs.oracle.com/cd/B28359_01/server.111/b28320/limits001.htm#i287903 
        // and http://docs.oracle.com/cd/B19306_01/server.102/b14220/datatype.htm#i13446
        // for limits in Oracle data types. 
        [Fact]
        public void AnsiStringDefaultIsVarchar2_255()
        {
            _typeMap.GetTypeMap(DbType.AnsiString, 0, 0).ShouldBe("VARCHAR2(255 CHAR)");
        }
        
        [Fact]
        public void AnsiStringOfSizeIsVarchar2OfSize()
        {
            _typeMap.GetTypeMap(DbType.AnsiString, 4000, 0).ShouldBe("VARCHAR2(4000 CHAR)");
        }

        [Fact]
        public void AnsiStringOver4000IsClob()
        {
            _typeMap.GetTypeMap(DbType.AnsiString, 4001, 0).ShouldBe("CLOB");
        }

        [Fact]
        public void AnsiStringFixedDefaultIsChar_255()
        {
            _typeMap.GetTypeMap(DbType.AnsiStringFixedLength, 0, 0).ShouldBe("CHAR(255 CHAR)");
        }

        [Fact]
        public void AnsiStringFixedOfSizeIsCharOfSize()
        {
            _typeMap.GetTypeMap(DbType.AnsiStringFixedLength, 2000, 0).ShouldBe("CHAR(2000 CHAR)");
        }


        [Fact]
        public void BinaryDefaultIsRaw_2000()
        {
            _typeMap.GetTypeMap(DbType.Binary, 0, 0).ShouldBe("RAW(2000)");
        }

        [Fact]
        public void BinaryOfSizeIsRawOfSize()
        {
            _typeMap.GetTypeMap(DbType.Binary, 2000, 0).ShouldBe("RAW(2000)");
        }


        [Fact]
        public void BinaryOver2000IsBlob()
        {
            _typeMap.GetTypeMap(DbType.Binary, 2001, 0).ShouldBe("BLOB");
        }

        [Fact]
        public void BooleanIsNumber()
        {
            _typeMap.GetTypeMap(DbType.Boolean, 0, 0).ShouldBe("NUMBER(1,0)");
        }

        [Fact]
        public void ByteIsNumber()
        {
            _typeMap.GetTypeMap(DbType.Byte, 0, 0).ShouldBe("NUMBER(3,0)");
        }

        [Fact]
        public void CurrencyIsNumber()
        {
            _typeMap.GetTypeMap(DbType.Currency, 0, 0).ShouldBe("NUMBER(19,4)");
        }

        [Fact]
        public void DateIsDate()
        {
            _typeMap.GetTypeMap(DbType.Date, 0, 0).ShouldBe("DATE");
        }

        [Fact]
        public void DateTimeIsTimestamp()
        {
            _typeMap.GetTypeMap(DbType.DateTime, 0, 0).ShouldBe("TIMESTAMP(4)");
        }

        [Fact]
        public void DateTimeOffsetIsTimestampWithTimeZone()
        {
            _typeMap.GetTypeMap(DbType.DateTimeOffset, 0, 0).ShouldBe("TIMESTAMP(4) WITH TIME ZONE");
        }

        [Fact]
        public void DecimalDefaultIsNumber()
        {
            _typeMap.GetTypeMap(DbType.Decimal, 0, 0).ShouldBe("NUMBER(19,5)");
        }

        [Fact]
        public void DecimalOfPrecisionIsNumberWithPrecision()
        {
            _typeMap.GetTypeMap(DbType.Decimal, 8, 3).ShouldBe("NUMBER(8,3)");
        }

        [Fact]
        public void DoubleIsDouble()
        {
            _typeMap.GetTypeMap(DbType.Double, 0, 0).ShouldBe("DOUBLE PRECISION");
        }

        [Fact]
        public void GuidIsRaw()
        {
            _typeMap.GetTypeMap(DbType.Guid, 0, 0).ShouldBe("RAW(16)");
        }

        [Fact]
        public void Int16IsNumber()
        {
            _typeMap.GetTypeMap(DbType.Int16, 0, 0).ShouldBe("NUMBER(5,0)");
        }

        [Fact]
        public void In32IsNumber()
        {
            _typeMap.GetTypeMap(DbType.Int32, 0, 0).ShouldBe("NUMBER(10,0)");
        }

        [Fact]
        public void Int64IsNumber()
        {
            _typeMap.GetTypeMap(DbType.Int64, 0, 0).ShouldBe("NUMBER(19,0)");
        }

        [Fact]
        public void SingleIsFloat()
        {
            _typeMap.GetTypeMap(DbType.Single, 0, 0).ShouldBe("FLOAT(24)");
        }

        [Fact]
        public void StringFixedLengthDefaultIsNChar_255()
        {
            _typeMap.GetTypeMap(DbType.StringFixedLength, 0, 0).ShouldBe("NCHAR(255)");
        }

        [Fact]
        public void StringFixedLengthOfSizeIsNCharOfSize()
        {
            _typeMap.GetTypeMap(DbType.StringFixedLength, 2000, 0).ShouldBe("NCHAR(2000)");
        }


        [Fact]
        public void StringDefaultIsNVarchar2_255()
        {
            _typeMap.GetTypeMap(DbType.String, 0, 0).ShouldBe("NVARCHAR2(255)");
        }

        [Fact]
        public void StringOfLengthIsNVarchar2fLength()
        {
            _typeMap.GetTypeMap(DbType.String, 4000, 0).ShouldBe("NVARCHAR2(4000)");
        }

        [Fact]
        public void TimeIsDate()
        {
            _typeMap.GetTypeMap(DbType.Time, 0, 0).ShouldBe("DATE");
        }

        [Fact]
        public void XmlIsXmltype()
        {
            _typeMap.GetTypeMap(DbType.Xml, 0, 0).ShouldBe("XMLTYPE");
        }
    }
}


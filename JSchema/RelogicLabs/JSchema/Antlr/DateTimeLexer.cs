//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace RelogicLabs.JSchema.Antlr {
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
internal partial class DateTimeLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		ERA=1, YEAR_NUMBER4=2, YEAR_NUMBER2=3, MONTH_NAME=4, MONTH_SHORT_NAME=5,
		MONTH_NUMBER2=6, MONTH_NUMBER=7, WEEKDAY_NAME=8, WEEKDAY_SHORT_NAME=9,
		DAY_NUMBER2=10, DAY_NUMBER=11, CLOCK_AM_PM=12, HOUR_NUMBER2=13, HOUR_NUMBER=14,
		MINUTE_NUMBER2=15, MINUTE_NUMBER=16, SECOND_NUMBER2=17, SECOND_NUMBER=18,
		FRACTION_NUMBER6=19, FRACTION_NUMBER5=20, FRACTION_NUMBER4=21, FRACTION_NUMBER3=22,
		FRACTION_NUMBER2=23, FRACTION_NUMBER1=24, FRACTION_NUMBER=25, UTC_OFFSET_TIME2=26,
		UTC_OFFSET_TIME1=27, UTC_OFFSET_HOUR=28, SYMBOL=29, WHITESPACE=30, TEXT=31;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"ERA", "YEAR_NUMBER4", "YEAR_NUMBER2", "MONTH_NAME", "MONTH_SHORT_NAME",
		"MONTH_NUMBER2", "MONTH_NUMBER", "WEEKDAY_NAME", "WEEKDAY_SHORT_NAME",
		"DAY_NUMBER2", "DAY_NUMBER", "CLOCK_AM_PM", "HOUR_NUMBER2", "HOUR_NUMBER",
		"MINUTE_NUMBER2", "MINUTE_NUMBER", "SECOND_NUMBER2", "SECOND_NUMBER",
		"FRACTION_NUMBER6", "FRACTION_NUMBER5", "FRACTION_NUMBER4", "FRACTION_NUMBER3",
		"FRACTION_NUMBER2", "FRACTION_NUMBER1", "FRACTION_NUMBER", "UTC_OFFSET_TIME2",
		"UTC_OFFSET_TIME1", "UTC_OFFSET_HOUR", "SYMBOL", "WHITESPACE", "TEXT"
	};


	public DateTimeLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public DateTimeLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'G'", "'YYYY'", "'YY'", "'MMMM'", "'MMM'", "'MM'", "'M'", "'DDDD'",
		"'DDD'", "'DD'", "'D'", "'t'", "'hh'", "'h'", "'mm'", "'m'", "'ss'", "'s'",
		"'ffffff'", "'fffff'", "'ffff'", "'fff'", "'ff'", "'f'", "'F'", "'ZZZ'",
		"'ZZ'", "'Z'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "ERA", "YEAR_NUMBER4", "YEAR_NUMBER2", "MONTH_NAME", "MONTH_SHORT_NAME",
		"MONTH_NUMBER2", "MONTH_NUMBER", "WEEKDAY_NAME", "WEEKDAY_SHORT_NAME",
		"DAY_NUMBER2", "DAY_NUMBER", "CLOCK_AM_PM", "HOUR_NUMBER2", "HOUR_NUMBER",
		"MINUTE_NUMBER2", "MINUTE_NUMBER", "SECOND_NUMBER2", "SECOND_NUMBER",
		"FRACTION_NUMBER6", "FRACTION_NUMBER5", "FRACTION_NUMBER4", "FRACTION_NUMBER3",
		"FRACTION_NUMBER2", "FRACTION_NUMBER1", "FRACTION_NUMBER", "UTC_OFFSET_TIME2",
		"UTC_OFFSET_TIME1", "UTC_OFFSET_HOUR", "SYMBOL", "WHITESPACE", "TEXT"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "DateTimeLexer.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static DateTimeLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,31,177,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,2,27,7,27,2,28,
		7,28,2,29,7,29,2,30,7,30,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,2,1,2,1,2,1,3,1,
		3,1,3,1,3,1,3,1,4,1,4,1,4,1,4,1,5,1,5,1,5,1,6,1,6,1,7,1,7,1,7,1,7,1,7,
		1,8,1,8,1,8,1,8,1,9,1,9,1,9,1,10,1,10,1,11,1,11,1,12,1,12,1,12,1,13,1,
		13,1,14,1,14,1,14,1,15,1,15,1,16,1,16,1,16,1,17,1,17,1,18,1,18,1,18,1,
		18,1,18,1,18,1,18,1,19,1,19,1,19,1,19,1,19,1,19,1,20,1,20,1,20,1,20,1,
		20,1,21,1,21,1,21,1,21,1,22,1,22,1,22,1,23,1,23,1,24,1,24,1,25,1,25,1,
		25,1,25,1,26,1,26,1,26,1,27,1,27,1,28,4,28,158,8,28,11,28,12,28,159,1,
		29,4,29,163,8,29,11,29,12,29,164,1,30,1,30,1,30,1,30,5,30,171,8,30,10,
		30,12,30,174,9,30,1,30,1,30,0,0,31,1,1,3,2,5,3,7,4,9,5,11,6,13,7,15,8,
		17,9,19,10,21,11,23,12,25,13,27,14,29,15,31,16,33,17,35,18,37,19,39,20,
		41,21,43,22,45,23,47,24,49,25,51,26,53,27,55,28,57,29,59,30,61,31,1,0,
		3,4,0,33,47,58,64,91,96,123,126,3,0,9,10,13,13,32,32,1,0,39,39,180,0,1,
		1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,7,1,0,0,0,0,9,1,0,0,0,0,11,1,0,0,0,0,
		13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,0,0,0,19,1,0,0,0,0,21,1,0,0,0,0,23,1,
		0,0,0,0,25,1,0,0,0,0,27,1,0,0,0,0,29,1,0,0,0,0,31,1,0,0,0,0,33,1,0,0,0,
		0,35,1,0,0,0,0,37,1,0,0,0,0,39,1,0,0,0,0,41,1,0,0,0,0,43,1,0,0,0,0,45,
		1,0,0,0,0,47,1,0,0,0,0,49,1,0,0,0,0,51,1,0,0,0,0,53,1,0,0,0,0,55,1,0,0,
		0,0,57,1,0,0,0,0,59,1,0,0,0,0,61,1,0,0,0,1,63,1,0,0,0,3,65,1,0,0,0,5,70,
		1,0,0,0,7,73,1,0,0,0,9,78,1,0,0,0,11,82,1,0,0,0,13,85,1,0,0,0,15,87,1,
		0,0,0,17,92,1,0,0,0,19,96,1,0,0,0,21,99,1,0,0,0,23,101,1,0,0,0,25,103,
		1,0,0,0,27,106,1,0,0,0,29,108,1,0,0,0,31,111,1,0,0,0,33,113,1,0,0,0,35,
		116,1,0,0,0,37,118,1,0,0,0,39,125,1,0,0,0,41,131,1,0,0,0,43,136,1,0,0,
		0,45,140,1,0,0,0,47,143,1,0,0,0,49,145,1,0,0,0,51,147,1,0,0,0,53,151,1,
		0,0,0,55,154,1,0,0,0,57,157,1,0,0,0,59,162,1,0,0,0,61,166,1,0,0,0,63,64,
		5,71,0,0,64,2,1,0,0,0,65,66,5,89,0,0,66,67,5,89,0,0,67,68,5,89,0,0,68,
		69,5,89,0,0,69,4,1,0,0,0,70,71,5,89,0,0,71,72,5,89,0,0,72,6,1,0,0,0,73,
		74,5,77,0,0,74,75,5,77,0,0,75,76,5,77,0,0,76,77,5,77,0,0,77,8,1,0,0,0,
		78,79,5,77,0,0,79,80,5,77,0,0,80,81,5,77,0,0,81,10,1,0,0,0,82,83,5,77,
		0,0,83,84,5,77,0,0,84,12,1,0,0,0,85,86,5,77,0,0,86,14,1,0,0,0,87,88,5,
		68,0,0,88,89,5,68,0,0,89,90,5,68,0,0,90,91,5,68,0,0,91,16,1,0,0,0,92,93,
		5,68,0,0,93,94,5,68,0,0,94,95,5,68,0,0,95,18,1,0,0,0,96,97,5,68,0,0,97,
		98,5,68,0,0,98,20,1,0,0,0,99,100,5,68,0,0,100,22,1,0,0,0,101,102,5,116,
		0,0,102,24,1,0,0,0,103,104,5,104,0,0,104,105,5,104,0,0,105,26,1,0,0,0,
		106,107,5,104,0,0,107,28,1,0,0,0,108,109,5,109,0,0,109,110,5,109,0,0,110,
		30,1,0,0,0,111,112,5,109,0,0,112,32,1,0,0,0,113,114,5,115,0,0,114,115,
		5,115,0,0,115,34,1,0,0,0,116,117,5,115,0,0,117,36,1,0,0,0,118,119,5,102,
		0,0,119,120,5,102,0,0,120,121,5,102,0,0,121,122,5,102,0,0,122,123,5,102,
		0,0,123,124,5,102,0,0,124,38,1,0,0,0,125,126,5,102,0,0,126,127,5,102,0,
		0,127,128,5,102,0,0,128,129,5,102,0,0,129,130,5,102,0,0,130,40,1,0,0,0,
		131,132,5,102,0,0,132,133,5,102,0,0,133,134,5,102,0,0,134,135,5,102,0,
		0,135,42,1,0,0,0,136,137,5,102,0,0,137,138,5,102,0,0,138,139,5,102,0,0,
		139,44,1,0,0,0,140,141,5,102,0,0,141,142,5,102,0,0,142,46,1,0,0,0,143,
		144,5,102,0,0,144,48,1,0,0,0,145,146,5,70,0,0,146,50,1,0,0,0,147,148,5,
		90,0,0,148,149,5,90,0,0,149,150,5,90,0,0,150,52,1,0,0,0,151,152,5,90,0,
		0,152,153,5,90,0,0,153,54,1,0,0,0,154,155,5,90,0,0,155,56,1,0,0,0,156,
		158,7,0,0,0,157,156,1,0,0,0,158,159,1,0,0,0,159,157,1,0,0,0,159,160,1,
		0,0,0,160,58,1,0,0,0,161,163,7,1,0,0,162,161,1,0,0,0,163,164,1,0,0,0,164,
		162,1,0,0,0,164,165,1,0,0,0,165,60,1,0,0,0,166,172,5,39,0,0,167,171,8,
		2,0,0,168,169,5,39,0,0,169,171,5,39,0,0,170,167,1,0,0,0,170,168,1,0,0,
		0,171,174,1,0,0,0,172,170,1,0,0,0,172,173,1,0,0,0,173,175,1,0,0,0,174,
		172,1,0,0,0,175,176,5,39,0,0,176,62,1,0,0,0,5,0,159,164,170,172,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace RelogicLabs.JSchema.Antlr
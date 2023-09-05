lexer grammar DateLexer;

ERA : 'G';

YEAR_NUM4 : 'YYYY';
YEAR_NUM2 : 'YY';

MONTH_NAME : 'MMMM';
MONTH_SHORT_NAME : 'MMM';
MONTH_NUM2 : 'MM';
MONTH_NUM : 'M';

WEEKDAY_NAME : 'DDDD';
WEEKDAY_SHORT_NAME : 'DDD';

DAY_NUM2 : 'DD';
DAY_NUM : 'D';

AM_PM : 't';

HOUR_NUM2 : 'hh';
HOUR_NUM : 'h';

MINUTE_NUM2 : 'mm';
MINUTE_NUM : 'm';

SECOND_NUM2 : 'ss';
SECOND_NUM : 's';

FRACTION_NUM06 : 'ffffff';
FRACTION_NUM05 : 'fffff';
FRACTION_NUM04 : 'ffff';
FRACTION_NUM03 : 'fff';
FRACTION_NUM02 : 'ff';
FRACTION_NUM01 : 'f';

FRACTION_NUM : 'F';

UTC_OFFSET_TIME2 : 'ZZZ';
UTC_OFFSET_TIME1 : 'ZZ';
UTC_OFFSET_HOUR : 'Z';

SYMBOL : [!-/:-@[-`{-~]+;
WHITESPACE : [\n\r\t ]+;

TEXT : '\'' ( ~'\'' | '\'\'')* '\'';


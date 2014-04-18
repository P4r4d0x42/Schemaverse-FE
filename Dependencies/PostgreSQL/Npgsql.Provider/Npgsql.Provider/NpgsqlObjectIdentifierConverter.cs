using System;
using Microsoft.VisualStudio.Data.Framework.AdoDotNet;
using Microsoft.VisualStudio.Data.Services;
using Microsoft.VisualStudio.Data.Services.SupportEntities;

namespace Npgsql.Provider
{
    internal class NpgsqlObjectIdentifierConverter : AdoDotNetObjectIdentifierConverter
    {
        // Fields
        private string[] _reservedWords;
        private int _serverMajorVersion = 2;
        private static UnicodeRange[] s_alphabeticUnicodeRanges20 = new UnicodeRange[] { 
        new UnicodeRange(0x41, 90), new UnicodeRange(0x61, 0x7a), new UnicodeRange(170, 170), new UnicodeRange(0xb5, 0xb5), new UnicodeRange(0xba, 0xba), new UnicodeRange(0xc0, 0xd6), new UnicodeRange(0xd8, 0xf6), new UnicodeRange(0xf8, 0x1f5), new UnicodeRange(0x1fa, 0x217), new UnicodeRange(0x250, 680), new UnicodeRange(0x2b0, 0x2b8), new UnicodeRange(0x2bb, 0x2c1), new UnicodeRange(0x2e0, 740), new UnicodeRange(890, 890), new UnicodeRange(0x386, 0x386), new UnicodeRange(0x388, 0x38a), 
        new UnicodeRange(0x38c, 0x38c), new UnicodeRange(910, 0x3a1), new UnicodeRange(0x3a3, 0x3ce), new UnicodeRange(0x3d0, 0x3d6), new UnicodeRange(0x3da, 0x3da), new UnicodeRange(0x3dc, 0x3dc), new UnicodeRange(990, 990), new UnicodeRange(0x3e0, 0x3e0), new UnicodeRange(0x3e2, 0x3f3), new UnicodeRange(0x401, 0x40c), new UnicodeRange(0x40e, 0x44f), new UnicodeRange(0x451, 0x45c), new UnicodeRange(0x45e, 0x481), new UnicodeRange(0x490, 0x4c4), new UnicodeRange(0x4c7, 0x4c8), new UnicodeRange(0x4cb, 0x4cc), 
        new UnicodeRange(0x4d0, 0x4eb), new UnicodeRange(0x4ee, 0x4f5), new UnicodeRange(0x4f8, 0x4f9), new UnicodeRange(0x531, 0x556), new UnicodeRange(0x559, 0x559), new UnicodeRange(0x561, 0x587), new UnicodeRange(0x5d0, 0x5ea), new UnicodeRange(0x5f0, 0x5f2), new UnicodeRange(0x621, 0x63a), new UnicodeRange(0x641, 0x652), new UnicodeRange(0x670, 0x6b7), new UnicodeRange(0x6ba, 0x6be), new UnicodeRange(0x6c0, 0x6ce), new UnicodeRange(0x6d0, 0x6d3), new UnicodeRange(0x6d5, 0x6dc), new UnicodeRange(0x6e1, 0x6e8), 
        new UnicodeRange(0x6ed, 0x6ed), new UnicodeRange(0x901, 0x903), new UnicodeRange(0x905, 0x939), new UnicodeRange(0x93d, 0x94c), new UnicodeRange(0x958, 0x963), new UnicodeRange(0x981, 0x983), new UnicodeRange(0x985, 0x98c), new UnicodeRange(0x98f, 0x990), new UnicodeRange(0x993, 0x9a8), new UnicodeRange(0x9aa, 0x9b0), new UnicodeRange(0x9b2, 0x9b2), new UnicodeRange(0x9b6, 0x9b9), new UnicodeRange(0x9be, 0x9c4), new UnicodeRange(0x9c7, 0x9c8), new UnicodeRange(0x9cb, 0x9cc), new UnicodeRange(0x9d7, 0x9d7), 
        new UnicodeRange(0x9dc, 0x9dd), new UnicodeRange(0x9df, 0x9e3), new UnicodeRange(0x9f0, 0x9f1), new UnicodeRange(0xa02, 0xa02), new UnicodeRange(0xa05, 0xa0a), new UnicodeRange(0xa0f, 0xa10), new UnicodeRange(0xa13, 0xa28), new UnicodeRange(0xa2a, 0xa30), new UnicodeRange(0xa32, 0xa33), new UnicodeRange(0xa35, 0xa36), new UnicodeRange(0xa38, 0xa39), new UnicodeRange(0xa3e, 0xa42), new UnicodeRange(0xa47, 0xa48), new UnicodeRange(0xa4b, 0xa4c), new UnicodeRange(0xa59, 0xa5c), new UnicodeRange(0xa5e, 0xa5e), 
        new UnicodeRange(0xa70, 0xa74), new UnicodeRange(0xa81, 0xa83), new UnicodeRange(0xa85, 0xa8b), new UnicodeRange(0xa8d, 0xa8d), new UnicodeRange(0xa8f, 0xa91), new UnicodeRange(0xa93, 0xaa8), new UnicodeRange(0xaaa, 0xab0), new UnicodeRange(0xab2, 0xab3), new UnicodeRange(0xab5, 0xab9), new UnicodeRange(0xabd, 0xac5), new UnicodeRange(0xac7, 0xac9), new UnicodeRange(0xacb, 0xacc), new UnicodeRange(0xae0, 0xae0), new UnicodeRange(0xb01, 0xb03), new UnicodeRange(0xb05, 0xb0c), new UnicodeRange(0xb0f, 0xb10), 
        new UnicodeRange(0xb13, 0xb28), new UnicodeRange(0xb2a, 0xb30), new UnicodeRange(0xb32, 0xb33), new UnicodeRange(0xb36, 0xb39), new UnicodeRange(0xb3d, 0xb43), new UnicodeRange(0xb47, 0xb48), new UnicodeRange(0xb4b, 0xb4c), new UnicodeRange(0xb56, 0xb57), new UnicodeRange(0xb5c, 0xb5d), new UnicodeRange(0xb5f, 0xb61), new UnicodeRange(0xb82, 0xb83), new UnicodeRange(0xb85, 0xb8a), new UnicodeRange(0xb8e, 0xb90), new UnicodeRange(0xb92, 0xb95), new UnicodeRange(0xb99, 0xb9a), new UnicodeRange(0xb9c, 0xb9c), 
        new UnicodeRange(0xb9e, 0xb9f), new UnicodeRange(0xba3, 0xba4), new UnicodeRange(0xba8, 0xbaa), new UnicodeRange(0xbae, 0xbb5), new UnicodeRange(0xbb7, 0xbb9), new UnicodeRange(0xbbe, 0xbc2), new UnicodeRange(0xbc6, 0xbc8), new UnicodeRange(0xbca, 0xbcc), new UnicodeRange(0xbd7, 0xbd7), new UnicodeRange(0xc01, 0xc03), new UnicodeRange(0xc05, 0xc0c), new UnicodeRange(0xc0e, 0xc10), new UnicodeRange(0xc12, 0xc28), new UnicodeRange(0xc2a, 0xc33), new UnicodeRange(0xc35, 0xc39), new UnicodeRange(0xc3e, 0xc44), 
        new UnicodeRange(0xc46, 0xc48), new UnicodeRange(0xc4a, 0xc4c), new UnicodeRange(0xc55, 0xc56), new UnicodeRange(0xc60, 0xc61), new UnicodeRange(0xc82, 0xc83), new UnicodeRange(0xc85, 0xc8c), new UnicodeRange(0xc8e, 0xc90), new UnicodeRange(0xc92, 0xca8), new UnicodeRange(0xcaa, 0xcb3), new UnicodeRange(0xcb5, 0xcb9), new UnicodeRange(0xcbe, 0xcc4), new UnicodeRange(0xcc6, 0xcc8), new UnicodeRange(0xcca, 0xccc), new UnicodeRange(0xcd5, 0xcd6), new UnicodeRange(0xcde, 0xcde), new UnicodeRange(0xce0, 0xce1), 
        new UnicodeRange(0xd02, 0xd03), new UnicodeRange(0xd05, 0xd0c), new UnicodeRange(0xd0e, 0xd10), new UnicodeRange(0xd12, 0xd28), new UnicodeRange(0xd2a, 0xd39), new UnicodeRange(0xd3e, 0xd43), new UnicodeRange(0xd46, 0xd48), new UnicodeRange(0xd4a, 0xd4c), new UnicodeRange(0xd57, 0xd57), new UnicodeRange(0xd60, 0xd61), new UnicodeRange(0xe01, 0xe2e), new UnicodeRange(0xe30, 0xe3a), new UnicodeRange(0xe40, 0xe45), new UnicodeRange(0xe47, 0xe47), new UnicodeRange(0xe4d, 0xe4d), new UnicodeRange(0xe81, 0xe82), 
        new UnicodeRange(0xe84, 0xe84), new UnicodeRange(0xe87, 0xe88), new UnicodeRange(0xe8a, 0xe8a), new UnicodeRange(0xe8d, 0xe8d), new UnicodeRange(0xe94, 0xe97), new UnicodeRange(0xe99, 0xe9f), new UnicodeRange(0xea1, 0xea3), new UnicodeRange(0xea5, 0xea5), new UnicodeRange(0xea7, 0xea7), new UnicodeRange(0xeaa, 0xeab), new UnicodeRange(0xead, 0xeae), new UnicodeRange(0xeb0, 0xeb9), new UnicodeRange(0xebb, 0xebd), new UnicodeRange(0xec0, 0xec4), new UnicodeRange(0xecd, 0xecd), new UnicodeRange(0xedc, 0xedd), 
        new UnicodeRange(0xf40, 0xf47), new UnicodeRange(0xf49, 0xf69), new UnicodeRange(0xf71, 0xf81), new UnicodeRange(0xf90, 0xf95), new UnicodeRange(0xf97, 0xf97), new UnicodeRange(0xf99, 0xfad), new UnicodeRange(0xfb1, 0xfb7), new UnicodeRange(0xfb9, 0xfb9), new UnicodeRange(0x10a0, 0x10c5), new UnicodeRange(0x10d0, 0x10f6), new UnicodeRange(0x1100, 0x1159), new UnicodeRange(0x115f, 0x11a2), new UnicodeRange(0x11a8, 0x11f9), new UnicodeRange(0x1e00, 0x1e9b), new UnicodeRange(0x1ea0, 0x1ef9), new UnicodeRange(0x1f00, 0x1f15), 
        new UnicodeRange(0x1f18, 0x1f1d), new UnicodeRange(0x1f20, 0x1f45), new UnicodeRange(0x1f48, 0x1f4d), new UnicodeRange(0x1f50, 0x1f57), new UnicodeRange(0x1f59, 0x1f59), new UnicodeRange(0x1f5b, 0x1f5b), new UnicodeRange(0x1f5d, 0x1f5d), new UnicodeRange(0x1f5f, 0x1f7d), new UnicodeRange(0x1f80, 0x1fb4), new UnicodeRange(0x1fb6, 0x1fbc), new UnicodeRange(0x1fbe, 0x1fbe), new UnicodeRange(0x1fc2, 0x1fc4), new UnicodeRange(0x1fc6, 0x1fcc), new UnicodeRange(0x1fd0, 0x1fd3), new UnicodeRange(0x1fd6, 0x1fdb), new UnicodeRange(0x1fe0, 0x1fec), 
        new UnicodeRange(0x1ff2, 0x1ff4), new UnicodeRange(0x1ff6, 0x1ffc), new UnicodeRange(0x207f, 0x207f), new UnicodeRange(0x2102, 0x2102), new UnicodeRange(0x2107, 0x2107), new UnicodeRange(0x210a, 0x2113), new UnicodeRange(0x2115, 0x2115), new UnicodeRange(0x2118, 0x211d), new UnicodeRange(0x2124, 0x2124), new UnicodeRange(0x2126, 0x2126), new UnicodeRange(0x2128, 0x2128), new UnicodeRange(0x212a, 0x2131), new UnicodeRange(0x2133, 0x2138), new UnicodeRange(0x2160, 0x2182), new UnicodeRange(0x3041, 0x3094), new UnicodeRange(0x30a1, 0x30fa), 
        new UnicodeRange(0x3105, 0x312c), new UnicodeRange(0x3131, 0x318e), new UnicodeRange(0xac00, 0xd7a3), new UnicodeRange(0xfb00, 0xfb06), new UnicodeRange(0xfb13, 0xfb17), new UnicodeRange(0xfb1f, 0xfb28), new UnicodeRange(0xfb2a, 0xfb36), new UnicodeRange(0xfb38, 0xfb3c), new UnicodeRange(0xfb3e, 0xfb3e), new UnicodeRange(0xfb40, 0xfb41), new UnicodeRange(0xfb43, 0xfb44), new UnicodeRange(0xfb46, 0xfbb1), new UnicodeRange(0xfbd3, 0xfd3d), new UnicodeRange(0xfd50, 0xfd8f), new UnicodeRange(0xfd92, 0xfdc7), new UnicodeRange(0xfdf0, 0xfdfb), 
        new UnicodeRange(0xfe70, 0xfe72), new UnicodeRange(0xfe74, 0xfe74), new UnicodeRange(0xfe76, 0xfefc), new UnicodeRange(0xff21, 0xff3a), new UnicodeRange(0xff41, 0xff5a), new UnicodeRange(0xff66, 0xff6f), new UnicodeRange(0xff71, 0xff9d), new UnicodeRange(0xffa0, 0xffbe), new UnicodeRange(0xffc2, 0xffc7), new UnicodeRange(0xffca, 0xffcf), new UnicodeRange(0xffd2, 0xffd7), new UnicodeRange(0xffda, 0xffdc)
     };
        private static UnicodeRange[] s_combiningUnicodeRanges20 = new UnicodeRange[] { 
        new UnicodeRange(0x300, 0x345), new UnicodeRange(0x360, 0x361), new UnicodeRange(0x483, 0x486), new UnicodeRange(0x591, 0x5a1), new UnicodeRange(0x5a3, 0x5b9), new UnicodeRange(0x5bb, 0x5bd), new UnicodeRange(0x5bf, 0x5bf), new UnicodeRange(0x5c1, 0x5c2), new UnicodeRange(0x5c4, 0x5c4), new UnicodeRange(0x64b, 0x652), new UnicodeRange(0x670, 0x670), new UnicodeRange(0x6d6, 0x6e4), new UnicodeRange(0x6e7, 0x6e8), new UnicodeRange(0x6ea, 0x6ed), new UnicodeRange(0x901, 0x903), new UnicodeRange(0x93c, 0x93c), 
        new UnicodeRange(0x93e, 0x94d), new UnicodeRange(0x951, 0x954), new UnicodeRange(0x962, 0x963), new UnicodeRange(0x981, 0x983), new UnicodeRange(0x9bc, 0x9bc), new UnicodeRange(0x9be, 0x9c4), new UnicodeRange(0x9c7, 0x9c8), new UnicodeRange(0x9cb, 0x9cd), new UnicodeRange(0x9d7, 0x9d7), new UnicodeRange(0x9e2, 0x9e3), new UnicodeRange(0xa02, 0xa02), new UnicodeRange(0xa3c, 0xa3c), new UnicodeRange(0xa3e, 0xa42), new UnicodeRange(0xa47, 0xa48), new UnicodeRange(0xa4b, 0xa4d), new UnicodeRange(0xa70, 0xa71), 
        new UnicodeRange(0xa81, 0xa83), new UnicodeRange(0xabc, 0xabc), new UnicodeRange(0xabe, 0xac5), new UnicodeRange(0xac7, 0xac9), new UnicodeRange(0xacb, 0xacd), new UnicodeRange(0xb01, 0xb03), new UnicodeRange(0xb3c, 0xb3c), new UnicodeRange(0xb3e, 0xb43), new UnicodeRange(0xb47, 0xb48), new UnicodeRange(0xb4b, 0xb4d), new UnicodeRange(0xb56, 0xb57), new UnicodeRange(0xb82, 0xb83), new UnicodeRange(0xbbe, 0xbc2), new UnicodeRange(0xbc6, 0xbc8), new UnicodeRange(0xbca, 0xbcd), new UnicodeRange(0xbd7, 0xbd7), 
        new UnicodeRange(0xc01, 0xc03), new UnicodeRange(0xc3e, 0xc44), new UnicodeRange(0xc46, 0xc48), new UnicodeRange(0xc4a, 0xc4d), new UnicodeRange(0xc55, 0xc56), new UnicodeRange(0xc82, 0xc83), new UnicodeRange(0xcbe, 0xcc4), new UnicodeRange(0xcc6, 0xcc8), new UnicodeRange(0xcca, 0xccd), new UnicodeRange(0xcd5, 0xcd6), new UnicodeRange(0xd02, 0xd03), new UnicodeRange(0xd3e, 0xd43), new UnicodeRange(0xd46, 0xd48), new UnicodeRange(0xd4a, 0xd4d), new UnicodeRange(0xd57, 0xd57), new UnicodeRange(0xe31, 0xe31), 
        new UnicodeRange(0xe34, 0xe3a), new UnicodeRange(0xe47, 0xe4e), new UnicodeRange(0xeb1, 0xeb1), new UnicodeRange(0xeb4, 0xeb9), new UnicodeRange(0xebb, 0xebc), new UnicodeRange(0xec8, 0xecd), new UnicodeRange(0xf18, 0xf19), new UnicodeRange(0xf35, 0xf35), new UnicodeRange(0xf37, 0xf37), new UnicodeRange(0xf39, 0xf39), new UnicodeRange(0xf3e, 0xf3f), new UnicodeRange(0xf71, 0xf84), new UnicodeRange(0xf86, 0xf8b), new UnicodeRange(0xf90, 0xf95), new UnicodeRange(0xf97, 0xf97), new UnicodeRange(0xf99, 0xfad), 
        new UnicodeRange(0xfb1, 0xfb7), new UnicodeRange(0xfb9, 0xfb9), new UnicodeRange(0x20d0, 0x20e1), new UnicodeRange(0x302a, 0x302f), new UnicodeRange(0x3099, 0x309a), new UnicodeRange(0xfb1e, 0xfb1e), new UnicodeRange(0xfe20, 0xfe23)
     };
        private static UnicodeRange[] s_decimalDigitUnicodeRanges219 = new UnicodeRange[] { new UnicodeRange( 0x30, 0x39 ), new UnicodeRange( 0x660, 0x669 ), new UnicodeRange( 0x6f0, 0x6f9 ), new UnicodeRange( 0x966, 0x96f ), new UnicodeRange( 0x9e6, 0x9ef ), new UnicodeRange( 0xa66, 0xa6f ), new UnicodeRange( 0xae6, 0xaef ), new UnicodeRange( 0xb66, 0xb6f ), new UnicodeRange( 0xbe7, 0xbef ), new UnicodeRange( 0xc66, 0xc6f ), new UnicodeRange( 0xce6, 0xcef ), new UnicodeRange( 0xd66, 0xd6f ), new UnicodeRange( 0xe50, 0xe59 ), new UnicodeRange( 0xed0, 0xed9 ), new UnicodeRange( 0xf20, 0xf29 ), new UnicodeRange( 0xff10, 0xff19 ) };
        private static UnicodeRange[] s_enclosingCombinerUnicodeRanges20 = new UnicodeRange[] { new UnicodeRange( 0x20dd, 0x20e0 ) };
        private static UnicodeRange[] s_extenderUnicodeRanges219 = new UnicodeRange[] { new UnicodeRange( 0xb7, 0xb7 ), new UnicodeRange( 720, 0x2d1 ), new UnicodeRange( 0x387, 0x387 ), new UnicodeRange( 0x640, 0x640 ), new UnicodeRange( 0xe46, 0xe46 ), new UnicodeRange( 0xec6, 0xec6 ), new UnicodeRange( 0x3005, 0x3005 ), new UnicodeRange( 0x3031, 0x3035 ), new UnicodeRange( 0x309d, 0x309e ), new UnicodeRange( 0x30fc, 0x30fe ), new UnicodeRange( 0xff70, 0xff70 ) };
        private static UnicodeRange[] s_idContinueUnicodeRanges32 = new UnicodeRange[] { 
        new UnicodeRange(0x30, 0x39), new UnicodeRange(0x5f, 0x5f), new UnicodeRange(0x300, 0x34f), new UnicodeRange(0x360, 0x36f), new UnicodeRange(0x483, 0x486), new UnicodeRange(0x591, 0x5a1), new UnicodeRange(0x5a3, 0x5b9), new UnicodeRange(0x5bb, 0x5bd), new UnicodeRange(0x5bf, 0x5bf), new UnicodeRange(0x5c1, 0x5c2), new UnicodeRange(0x5c4, 0x5c4), new UnicodeRange(0x64b, 0x655), new UnicodeRange(0x660, 0x669), new UnicodeRange(0x670, 0x670), new UnicodeRange(0x6d6, 0x6dc), new UnicodeRange(0x6df, 0x6e4), 
        new UnicodeRange(0x6e7, 0x6e8), new UnicodeRange(0x6ea, 0x6ed), new UnicodeRange(0x6f0, 0x6f9), new UnicodeRange(0x711, 0x711), new UnicodeRange(0x730, 0x74a), new UnicodeRange(0x7a6, 0x7b0), new UnicodeRange(0x901, 0x902), new UnicodeRange(0x903, 0x903), new UnicodeRange(0x93c, 0x93c), new UnicodeRange(0x93e, 0x940), new UnicodeRange(0x941, 0x948), new UnicodeRange(0x949, 0x94c), new UnicodeRange(0x94d, 0x94d), new UnicodeRange(0x951, 0x954), new UnicodeRange(0x962, 0x963), new UnicodeRange(0x966, 0x96f), 
        new UnicodeRange(0x981, 0x981), new UnicodeRange(0x982, 0x983), new UnicodeRange(0x9bc, 0x9bc), new UnicodeRange(0x9be, 0x9c0), new UnicodeRange(0x9c1, 0x9c4), new UnicodeRange(0x9c7, 0x9c8), new UnicodeRange(0x9cb, 0x9cc), new UnicodeRange(0x9cd, 0x9cd), new UnicodeRange(0x9d7, 0x9d7), new UnicodeRange(0x9e2, 0x9e3), new UnicodeRange(0x9e6, 0x9ef), new UnicodeRange(0xa02, 0xa02), new UnicodeRange(0xa3c, 0xa3c), new UnicodeRange(0xa3e, 0xa40), new UnicodeRange(0xa41, 0xa42), new UnicodeRange(0xa47, 0xa48), 
        new UnicodeRange(0xa4b, 0xa4d), new UnicodeRange(0xa66, 0xa6f), new UnicodeRange(0xa70, 0xa71), new UnicodeRange(0xa81, 0xa82), new UnicodeRange(0xa83, 0xa83), new UnicodeRange(0xabc, 0xabc), new UnicodeRange(0xabe, 0xac0), new UnicodeRange(0xac1, 0xac5), new UnicodeRange(0xac7, 0xac8), new UnicodeRange(0xac9, 0xac9), new UnicodeRange(0xacb, 0xacc), new UnicodeRange(0xacd, 0xacd), new UnicodeRange(0xae6, 0xaef), new UnicodeRange(0xb01, 0xb01), new UnicodeRange(0xb02, 0xb03), new UnicodeRange(0xb3c, 0xb3c), 
        new UnicodeRange(0xb3e, 0xb3e), new UnicodeRange(0xb3f, 0xb3f), new UnicodeRange(0xb40, 0xb40), new UnicodeRange(0xb41, 0xb43), new UnicodeRange(0xb47, 0xb48), new UnicodeRange(0xb4b, 0xb4c), new UnicodeRange(0xb4d, 0xb4d), new UnicodeRange(0xb56, 0xb56), new UnicodeRange(0xb57, 0xb57), new UnicodeRange(0xb66, 0xb6f), new UnicodeRange(0xb82, 0xb82), new UnicodeRange(0xbbe, 0xbbf), new UnicodeRange(0xbc0, 0xbc0), new UnicodeRange(0xbc1, 0xbc2), new UnicodeRange(0xbc6, 0xbc8), new UnicodeRange(0xbca, 0xbcc), 
        new UnicodeRange(0xbcd, 0xbcd), new UnicodeRange(0xbd7, 0xbd7), new UnicodeRange(0xbe7, 0xbef), new UnicodeRange(0xc01, 0xc03), new UnicodeRange(0xc3e, 0xc40), new UnicodeRange(0xc41, 0xc44), new UnicodeRange(0xc46, 0xc48), new UnicodeRange(0xc4a, 0xc4d), new UnicodeRange(0xc55, 0xc56), new UnicodeRange(0xc66, 0xc6f), new UnicodeRange(0xc82, 0xc83), new UnicodeRange(0xcbe, 0xcbe), new UnicodeRange(0xcbf, 0xcbf), new UnicodeRange(0xcc0, 0xcc4), new UnicodeRange(0xcc6, 0xcc6), new UnicodeRange(0xcc7, 0xcc8), 
        new UnicodeRange(0xcca, 0xccb), new UnicodeRange(0xccc, 0xccd), new UnicodeRange(0xcd5, 0xcd6), new UnicodeRange(0xce6, 0xcef), new UnicodeRange(0xd02, 0xd03), new UnicodeRange(0xd3e, 0xd40), new UnicodeRange(0xd41, 0xd43), new UnicodeRange(0xd46, 0xd48), new UnicodeRange(0xd4a, 0xd4c), new UnicodeRange(0xd4d, 0xd4d), new UnicodeRange(0xd57, 0xd57), new UnicodeRange(0xd66, 0xd6f), new UnicodeRange(0xd82, 0xd83), new UnicodeRange(0xdca, 0xdca), new UnicodeRange(0xdcf, 0xdd1), new UnicodeRange(0xdd2, 0xdd4), 
        new UnicodeRange(0xdd6, 0xdd6), new UnicodeRange(0xdd8, 0xddf), new UnicodeRange(0xdf2, 0xdf3), new UnicodeRange(0xe31, 0xe31), new UnicodeRange(0xe34, 0xe3a), new UnicodeRange(0xe47, 0xe4e), new UnicodeRange(0xe50, 0xe59), new UnicodeRange(0xeb1, 0xeb1), new UnicodeRange(0xeb4, 0xeb9), new UnicodeRange(0xebb, 0xebc), new UnicodeRange(0xec8, 0xecd), new UnicodeRange(0xed0, 0xed9), new UnicodeRange(0xf18, 0xf19), new UnicodeRange(0xf20, 0xf29), new UnicodeRange(0xf35, 0xf35), new UnicodeRange(0xf37, 0xf37), 
        new UnicodeRange(0xf39, 0xf39), new UnicodeRange(0xf3e, 0xf3f), new UnicodeRange(0xf71, 0xf7e), new UnicodeRange(0xf7f, 0xf7f), new UnicodeRange(0xf80, 0xf84), new UnicodeRange(0xf86, 0xf87), new UnicodeRange(0xf90, 0xf97), new UnicodeRange(0xf99, 0xfbc), new UnicodeRange(0xfc6, 0xfc6), new UnicodeRange(0x102c, 0x102c), new UnicodeRange(0x102d, 0x1030), new UnicodeRange(0x1031, 0x1031), new UnicodeRange(0x1032, 0x1032), new UnicodeRange(0x1036, 0x1037), new UnicodeRange(0x1038, 0x1038), new UnicodeRange(0x1039, 0x1039), 
        new UnicodeRange(0x1040, 0x1049), new UnicodeRange(0x1056, 0x1057), new UnicodeRange(0x1058, 0x1059), new UnicodeRange(0x1369, 0x1371), new UnicodeRange(0x1712, 0x1714), new UnicodeRange(0x1732, 0x1734), new UnicodeRange(0x1752, 0x1753), new UnicodeRange(0x1772, 0x1773), new UnicodeRange(0x17b4, 0x17b6), new UnicodeRange(0x17b7, 0x17bd), new UnicodeRange(0x17be, 0x17c5), new UnicodeRange(0x17c6, 0x17c6), new UnicodeRange(0x17c7, 0x17c8), new UnicodeRange(0x17c9, 0x17d3), new UnicodeRange(0x17e0, 0x17e9), new UnicodeRange(0x180b, 0x180d), 
        new UnicodeRange(0x1810, 0x1819), new UnicodeRange(0x18a9, 0x18a9), new UnicodeRange(0x203f, 0x2040), new UnicodeRange(0x20d0, 0x20dc), new UnicodeRange(0x20e1, 0x20e1), new UnicodeRange(0x20e5, 0x20ea), new UnicodeRange(0x302a, 0x302f), new UnicodeRange(0x3099, 0x309a), new UnicodeRange(0x30fb, 0x30fb), new UnicodeRange(0xfb1e, 0xfb1e), new UnicodeRange(0xfe00, 0xfe0f), new UnicodeRange(0xfe20, 0xfe23), new UnicodeRange(0xfe33, 0xfe34), new UnicodeRange(0xfe4d, 0xfe4f), new UnicodeRange(0xff10, 0xff19), new UnicodeRange(0xff3f, 0xff3f), 
        new UnicodeRange(0xff65, 0xff65), new UnicodeRange(0x1d165, 0x1d166), new UnicodeRange(0x1d167, 0x1d169), new UnicodeRange(0x1d16d, 0x1d172), new UnicodeRange(0x1d17b, 0x1d182), new UnicodeRange(0x1d185, 0x1d18b), new UnicodeRange(0x1d1aa, 0x1d1ad), new UnicodeRange(0x1d7ce, 0x1d7ff)
     };
        private static UnicodeRange[] s_ideographicUnicodeRanges20 = new UnicodeRange[] { new UnicodeRange( 0x3007, 0x3007 ), new UnicodeRange( 0x3021, 0x3029 ), new UnicodeRange( 0x4e00, 0x9fa5 ), new UnicodeRange( 0xf900, 0xfa2d ) };
        private static UnicodeRange[] s_idOtherFormatUnicodeRanges32 = new UnicodeRange[] { new UnicodeRange( 0x200c, 0x200f ), new UnicodeRange( 0x202a, 0x202e ), new UnicodeRange( 0x2060, 0x2063 ), new UnicodeRange( 0x206a, 0x206f ), new UnicodeRange( 0xfeff, 0xfeff ), new UnicodeRange( 0xfff9, 0xfffb ), new UnicodeRange( 0xe0001, 0xe0001 ), new UnicodeRange( 0xe0020, 0xe007f ) };
        private static UnicodeRange[] s_idStartUnicodeRanges32 = new UnicodeRange[] { 
        new UnicodeRange(0x41, 90), new UnicodeRange(0x61, 0x7a), new UnicodeRange(170, 170), new UnicodeRange(0xb5, 0xb5), new UnicodeRange(0xba, 0xba), new UnicodeRange(0xc0, 0xd6), new UnicodeRange(0xd8, 0xf6), new UnicodeRange(0xf8, 0x1ba), new UnicodeRange(0x1bb, 0x1bb), new UnicodeRange(0x1bc, 0x1bf), new UnicodeRange(0x1c0, 0x1c3), new UnicodeRange(0x1c4, 0x220), new UnicodeRange(0x222, 0x233), new UnicodeRange(0x250, 0x2ad), new UnicodeRange(0x2b0, 0x2b8), new UnicodeRange(0x2bb, 0x2c1), 
        new UnicodeRange(720, 0x2d1), new UnicodeRange(0x2e0, 740), new UnicodeRange(750, 750), new UnicodeRange(890, 890), new UnicodeRange(0x386, 0x386), new UnicodeRange(0x388, 0x38a), new UnicodeRange(0x38c, 0x38c), new UnicodeRange(910, 0x3a1), new UnicodeRange(0x3a3, 0x3ce), new UnicodeRange(0x3d0, 0x3f5), new UnicodeRange(0x400, 0x481), new UnicodeRange(0x48a, 0x4ce), new UnicodeRange(0x4d0, 0x4f5), new UnicodeRange(0x4f8, 0x4f9), new UnicodeRange(0x500, 0x50f), new UnicodeRange(0x531, 0x556), 
        new UnicodeRange(0x559, 0x559), new UnicodeRange(0x561, 0x587), new UnicodeRange(0x5d0, 0x5ea), new UnicodeRange(0x5f0, 0x5f2), new UnicodeRange(0x621, 0x63a), new UnicodeRange(0x640, 0x640), new UnicodeRange(0x641, 0x64a), new UnicodeRange(0x66e, 0x66f), new UnicodeRange(0x671, 0x6d3), new UnicodeRange(0x6d5, 0x6d5), new UnicodeRange(0x6e5, 0x6e6), new UnicodeRange(0x6fa, 0x6fc), new UnicodeRange(0x710, 0x710), new UnicodeRange(0x712, 0x72c), new UnicodeRange(0x780, 0x7a5), new UnicodeRange(0x7b1, 0x7b1), 
        new UnicodeRange(0x905, 0x939), new UnicodeRange(0x93d, 0x93d), new UnicodeRange(0x950, 0x950), new UnicodeRange(0x958, 0x961), new UnicodeRange(0x985, 0x98c), new UnicodeRange(0x98f, 0x990), new UnicodeRange(0x993, 0x9a8), new UnicodeRange(0x9aa, 0x9b0), new UnicodeRange(0x9b2, 0x9b2), new UnicodeRange(0x9b6, 0x9b9), new UnicodeRange(0x9dc, 0x9dd), new UnicodeRange(0x9df, 0x9e1), new UnicodeRange(0x9f0, 0x9f1), new UnicodeRange(0xa05, 0xa0a), new UnicodeRange(0xa0f, 0xa10), new UnicodeRange(0xa13, 0xa28), 
        new UnicodeRange(0xa2a, 0xa30), new UnicodeRange(0xa32, 0xa33), new UnicodeRange(0xa35, 0xa36), new UnicodeRange(0xa38, 0xa39), new UnicodeRange(0xa59, 0xa5c), new UnicodeRange(0xa5e, 0xa5e), new UnicodeRange(0xa72, 0xa74), new UnicodeRange(0xa85, 0xa8b), new UnicodeRange(0xa8d, 0xa8d), new UnicodeRange(0xa8f, 0xa91), new UnicodeRange(0xa93, 0xaa8), new UnicodeRange(0xaaa, 0xab0), new UnicodeRange(0xab2, 0xab3), new UnicodeRange(0xab5, 0xab9), new UnicodeRange(0xabd, 0xabd), new UnicodeRange(0xad0, 0xad0), 
        new UnicodeRange(0xae0, 0xae0), new UnicodeRange(0xb05, 0xb0c), new UnicodeRange(0xb0f, 0xb10), new UnicodeRange(0xb13, 0xb28), new UnicodeRange(0xb2a, 0xb30), new UnicodeRange(0xb32, 0xb33), new UnicodeRange(0xb36, 0xb39), new UnicodeRange(0xb3d, 0xb3d), new UnicodeRange(0xb5c, 0xb5d), new UnicodeRange(0xb5f, 0xb61), new UnicodeRange(0xb83, 0xb83), new UnicodeRange(0xb85, 0xb8a), new UnicodeRange(0xb8e, 0xb90), new UnicodeRange(0xb92, 0xb95), new UnicodeRange(0xb99, 0xb9a), new UnicodeRange(0xb9c, 0xb9c), 
        new UnicodeRange(0xb9e, 0xb9f), new UnicodeRange(0xba3, 0xba4), new UnicodeRange(0xba8, 0xbaa), new UnicodeRange(0xbae, 0xbb5), new UnicodeRange(0xbb7, 0xbb9), new UnicodeRange(0xc05, 0xc0c), new UnicodeRange(0xc0e, 0xc10), new UnicodeRange(0xc12, 0xc28), new UnicodeRange(0xc2a, 0xc33), new UnicodeRange(0xc35, 0xc39), new UnicodeRange(0xc60, 0xc61), new UnicodeRange(0xc85, 0xc8c), new UnicodeRange(0xc8e, 0xc90), new UnicodeRange(0xc92, 0xca8), new UnicodeRange(0xcaa, 0xcb3), new UnicodeRange(0xcb5, 0xcb9), 
        new UnicodeRange(0xcde, 0xcde), new UnicodeRange(0xce0, 0xce1), new UnicodeRange(0xd05, 0xd0c), new UnicodeRange(0xd0e, 0xd10), new UnicodeRange(0xd12, 0xd28), new UnicodeRange(0xd2a, 0xd39), new UnicodeRange(0xd60, 0xd61), new UnicodeRange(0xd85, 0xd96), new UnicodeRange(0xd9a, 0xdb1), new UnicodeRange(0xdb3, 0xdbb), new UnicodeRange(0xdbd, 0xdbd), new UnicodeRange(0xdc0, 0xdc6), new UnicodeRange(0xe01, 0xe30), new UnicodeRange(0xe32, 0xe33), new UnicodeRange(0xe40, 0xe45), new UnicodeRange(0xe46, 0xe46), 
        new UnicodeRange(0xe81, 0xe82), new UnicodeRange(0xe84, 0xe84), new UnicodeRange(0xe87, 0xe88), new UnicodeRange(0xe8a, 0xe8a), new UnicodeRange(0xe8d, 0xe8d), new UnicodeRange(0xe94, 0xe97), new UnicodeRange(0xe99, 0xe9f), new UnicodeRange(0xea1, 0xea3), new UnicodeRange(0xea5, 0xea5), new UnicodeRange(0xea7, 0xea7), new UnicodeRange(0xeaa, 0xeab), new UnicodeRange(0xead, 0xeb0), new UnicodeRange(0xeb2, 0xeb3), new UnicodeRange(0xebd, 0xebd), new UnicodeRange(0xec0, 0xec4), new UnicodeRange(0xec6, 0xec6), 
        new UnicodeRange(0xedc, 0xedd), new UnicodeRange(0xf00, 0xf00), new UnicodeRange(0xf40, 0xf47), new UnicodeRange(0xf49, 0xf6a), new UnicodeRange(0xf88, 0xf8b), new UnicodeRange(0x1000, 0x1021), new UnicodeRange(0x1023, 0x1027), new UnicodeRange(0x1029, 0x102a), new UnicodeRange(0x1050, 0x1055), new UnicodeRange(0x10a0, 0x10c5), new UnicodeRange(0x10d0, 0x10f8), new UnicodeRange(0x1100, 0x1159), new UnicodeRange(0x115f, 0x11a2), new UnicodeRange(0x11a8, 0x11f9), new UnicodeRange(0x1200, 0x1206), new UnicodeRange(0x1208, 0x1246), 
        new UnicodeRange(0x1248, 0x1248), new UnicodeRange(0x124a, 0x124d), new UnicodeRange(0x1250, 0x1256), new UnicodeRange(0x1258, 0x1258), new UnicodeRange(0x125a, 0x125d), new UnicodeRange(0x1260, 0x1286), new UnicodeRange(0x1288, 0x1288), new UnicodeRange(0x128a, 0x128d), new UnicodeRange(0x1290, 0x12ae), new UnicodeRange(0x12b0, 0x12b0), new UnicodeRange(0x12b2, 0x12b5), new UnicodeRange(0x12b8, 0x12be), new UnicodeRange(0x12c0, 0x12c0), new UnicodeRange(0x12c2, 0x12c5), new UnicodeRange(0x12c8, 0x12ce), new UnicodeRange(0x12d0, 0x12d6), 
        new UnicodeRange(0x12d8, 0x12ee), new UnicodeRange(0x12f0, 0x130e), new UnicodeRange(0x1310, 0x1310), new UnicodeRange(0x1312, 0x1315), new UnicodeRange(0x1318, 0x131e), new UnicodeRange(0x1320, 0x1346), new UnicodeRange(0x1348, 0x135a), new UnicodeRange(0x13a0, 0x13f4), new UnicodeRange(0x1401, 0x166c), new UnicodeRange(0x166f, 0x1676), new UnicodeRange(0x1681, 0x169a), new UnicodeRange(0x16a0, 0x16ea), new UnicodeRange(0x16ee, 0x16f0), new UnicodeRange(0x1700, 0x170c), new UnicodeRange(0x170e, 0x1711), new UnicodeRange(0x1720, 0x1731), 
        new UnicodeRange(0x1740, 0x1751), new UnicodeRange(0x1760, 0x176c), new UnicodeRange(0x176e, 0x1770), new UnicodeRange(0x1780, 0x17b3), new UnicodeRange(0x17d7, 0x17d7), new UnicodeRange(0x17dc, 0x17dc), new UnicodeRange(0x1820, 0x1842), new UnicodeRange(0x1843, 0x1843), new UnicodeRange(0x1844, 0x1877), new UnicodeRange(0x1880, 0x18a8), new UnicodeRange(0x1e00, 0x1e9b), new UnicodeRange(0x1ea0, 0x1ef9), new UnicodeRange(0x1f00, 0x1f15), new UnicodeRange(0x1f18, 0x1f1d), new UnicodeRange(0x1f20, 0x1f45), new UnicodeRange(0x1f48, 0x1f4d), 
        new UnicodeRange(0x1f50, 0x1f57), new UnicodeRange(0x1f59, 0x1f59), new UnicodeRange(0x1f5b, 0x1f5b), new UnicodeRange(0x1f5d, 0x1f5d), new UnicodeRange(0x1f5f, 0x1f7d), new UnicodeRange(0x1f80, 0x1fb4), new UnicodeRange(0x1fb6, 0x1fbc), new UnicodeRange(0x1fbe, 0x1fbe), new UnicodeRange(0x1fc2, 0x1fc4), new UnicodeRange(0x1fc6, 0x1fcc), new UnicodeRange(0x1fd0, 0x1fd3), new UnicodeRange(0x1fd6, 0x1fdb), new UnicodeRange(0x1fe0, 0x1fec), new UnicodeRange(0x1ff2, 0x1ff4), new UnicodeRange(0x1ff6, 0x1ffc), new UnicodeRange(0x2071, 0x2071), 
        new UnicodeRange(0x207f, 0x207f), new UnicodeRange(0x2102, 0x2102), new UnicodeRange(0x2107, 0x2107), new UnicodeRange(0x210a, 0x2113), new UnicodeRange(0x2115, 0x2115), new UnicodeRange(0x2119, 0x211d), new UnicodeRange(0x2124, 0x2124), new UnicodeRange(0x2126, 0x2126), new UnicodeRange(0x2128, 0x2128), new UnicodeRange(0x212a, 0x212d), new UnicodeRange(0x212f, 0x2131), new UnicodeRange(0x2133, 0x2134), new UnicodeRange(0x2135, 0x2138), new UnicodeRange(0x2139, 0x2139), new UnicodeRange(0x213d, 0x213f), new UnicodeRange(0x2145, 0x2149), 
        new UnicodeRange(0x2160, 0x2183), new UnicodeRange(0x3005, 0x3005), new UnicodeRange(0x3006, 0x3006), new UnicodeRange(0x3007, 0x3007), new UnicodeRange(0x3021, 0x3029), new UnicodeRange(0x3031, 0x3035), new UnicodeRange(0x3038, 0x303a), new UnicodeRange(0x303b, 0x303b), new UnicodeRange(0x303c, 0x303c), new UnicodeRange(0x3041, 0x3096), new UnicodeRange(0x309d, 0x309e), new UnicodeRange(0x309f, 0x309f), new UnicodeRange(0x30a1, 0x30fa), new UnicodeRange(0x30fc, 0x30fe), new UnicodeRange(0x30ff, 0x30ff), new UnicodeRange(0x3105, 0x312c), 
        new UnicodeRange(0x3131, 0x318e), new UnicodeRange(0x31a0, 0x31b7), new UnicodeRange(0x31f0, 0x31ff), new UnicodeRange(0x3400, 0x4db5), new UnicodeRange(0x4e00, 0x9fa5), new UnicodeRange(0xa000, 0xa48c), new UnicodeRange(0xac00, 0xd7a3), new UnicodeRange(0xf900, 0xfa2d), new UnicodeRange(0xfa30, 0xfa6a), new UnicodeRange(0xfb00, 0xfb06), new UnicodeRange(0xfb13, 0xfb17), new UnicodeRange(0xfb1d, 0xfb1d), new UnicodeRange(0xfb1f, 0xfb28), new UnicodeRange(0xfb2a, 0xfb36), new UnicodeRange(0xfb38, 0xfb3c), new UnicodeRange(0xfb3e, 0xfb3e), 
        new UnicodeRange(0xfb40, 0xfb41), new UnicodeRange(0xfb43, 0xfb44), new UnicodeRange(0xfb46, 0xfbb1), new UnicodeRange(0xfbd3, 0xfd3d), new UnicodeRange(0xfd50, 0xfd8f), new UnicodeRange(0xfd92, 0xfdc7), new UnicodeRange(0xfdf0, 0xfdfb), new UnicodeRange(0xfe70, 0xfe74), new UnicodeRange(0xfe76, 0xfefc), new UnicodeRange(0xff21, 0xff3a), new UnicodeRange(0xff41, 0xff5a), new UnicodeRange(0xff66, 0xff6f), new UnicodeRange(0xff70, 0xff70), new UnicodeRange(0xff71, 0xff9d), new UnicodeRange(0xff9e, 0xff9f), new UnicodeRange(0xffa0, 0xffbe), 
        new UnicodeRange(0xffc2, 0xffc7), new UnicodeRange(0xffca, 0xffcf), new UnicodeRange(0xffd2, 0xffd7), new UnicodeRange(0xffda, 0xffdc), new UnicodeRange(0x10300, 0x1031e), new UnicodeRange(0x10330, 0x10349), new UnicodeRange(0x1034a, 0x1034a), new UnicodeRange(0x10400, 0x10425), new UnicodeRange(0x10428, 0x1044d), new UnicodeRange(0x1d400, 0x1d454), new UnicodeRange(0x1d456, 0x1d49c), new UnicodeRange(0x1d49e, 0x1d49f), new UnicodeRange(0x1d4a2, 0x1d4a2), new UnicodeRange(0x1d4a5, 0x1d4a6), new UnicodeRange(0x1d4a9, 0x1d4ac), new UnicodeRange(0x1d4ae, 0x1d4b9), 
        new UnicodeRange(0x1d4bb, 0x1d4bb), new UnicodeRange(0x1d4bd, 0x1d4c0), new UnicodeRange(0x1d4c2, 0x1d4c3), new UnicodeRange(0x1d4c5, 0x1d505), new UnicodeRange(0x1d507, 0x1d50a), new UnicodeRange(0x1d50d, 0x1d514), new UnicodeRange(0x1d516, 0x1d51c), new UnicodeRange(0x1d51e, 0x1d539), new UnicodeRange(0x1d53b, 0x1d53e), new UnicodeRange(0x1d540, 0x1d544), new UnicodeRange(0x1d546, 0x1d546), new UnicodeRange(0x1d54a, 0x1d550), new UnicodeRange(0x1d552, 0x1d6a3), new UnicodeRange(0x1d6a8, 0x1d6c0), new UnicodeRange(0x1d6c2, 0x1d6da), new UnicodeRange(0x1d6dc, 0x1d6fa), 
        new UnicodeRange(0x1d6fc, 0x1d714), new UnicodeRange(0x1d716, 0x1d734), new UnicodeRange(0x1d736, 0x1d74e), new UnicodeRange(0x1d750, 0x1d76e), new UnicodeRange(0x1d770, 0x1d788), new UnicodeRange(0x1d78a, 0x1d7a8), new UnicodeRange(0x1d7aa, 0x1d7c2), new UnicodeRange(0x1d7c4, 0x1d7c9), new UnicodeRange(0x20000, 0x2a6d6), new UnicodeRange(0x2f800, 0x2fa1d)
     };
        private static UnicodeRange[] s_ignorableUnicodeRanges219 = new UnicodeRange[] { new UnicodeRange( 0x200c, 0x200f ), new UnicodeRange( 0x202a, 0x202e ), new UnicodeRange( 0x206a, 0x206f ), new UnicodeRange( 0xfeff, 0xfeff ) };
        
        // Methods
        protected override string BuildString( string typeName, string[] identifierParts, DataObjectIdentifierFormat format )
        {
            if ( string.Equals( typeName, "Diagram", StringComparison.OrdinalIgnoreCase ) && ( ( format & DataObjectIdentifierFormat.ForDisplay ) != DataObjectIdentifierFormat.None ) )
            {
                return identifierParts[ 3 ];
            }
            return base.BuildString( typeName, identifierParts, format );
        }

        private static bool ExistsInRanges( UnicodeRange[] ranges, int code )
        {
            foreach ( UnicodeRange range in ranges )
            {
                if ( ( range.LowCode <= code ) && ( code <= range.HighCode ) )
                {
                    return true;
                }
            }
            return false;
        }
        
        protected override string FormatPart( string typeName, object identifierPart, DataObjectIdentifierFormat format )
        {
            if ( ( identifierPart == null ) || ( identifierPart is DBNull ) )
            {
                return null;
            }
            string str = identifierPart.ToString();
            if ( ( ( format & DataObjectIdentifierFormat.WithQuotes ) != DataObjectIdentifierFormat.None ) && this.RequiresQuoting( str ) /*( typeName == "Table" || typeName == "Column" || typeName == "Function" )*/ )
            {
                return ( "\"" + str.Replace( "\"", "\"\"" ) + "\"" );
            }
            return str;
        }

        private static bool MustQuoteOnSql8OrBelow( char c, bool isFirstCharacter )
        {
            int code = c;
            if ( ( ExistsInRanges( s_alphabeticUnicodeRanges20, code ) && !ExistsInRanges( s_combiningUnicodeRanges20, code ) ) || ExistsInRanges( s_ideographicUnicodeRanges20, code ) )
            {
                return false;
            }
            if ( isFirstCharacter )
            {
                return true;
            }
            if ( ExistsInRanges( s_decimalDigitUnicodeRanges219, code ) )
            {
                return false;
            }
            if ( ( ExistsInRanges( s_combiningUnicodeRanges20, code ) && !ExistsInRanges( s_enclosingCombinerUnicodeRanges20, code ) ) && ( ( 0xf88 > code ) || ( code > 0xf8b ) ) )
            {
                return false;
            }
            if ( ExistsInRanges( s_extenderUnicodeRanges219, code ) )
            {
                return false;
            }
            if ( ExistsInRanges( s_ignorableUnicodeRanges219, code ) )
            {
                return false;
            }
            return ( ( ( c != '@' ) && ( c != '#' ) ) && ( ( ( c != '$' ) && ( c != '_' ) ) && ( code != 0xff3f ) ) );
        }

        private static bool MustQuoteOnSql9OrHigher( char c, bool isFirstCharacter )
        {
            int code = c;
            if ( isFirstCharacter )
            {
                return !ExistsInRanges( s_idStartUnicodeRanges32, code );
            }
            if ( ExistsInRanges( s_idStartUnicodeRanges32, code ) || ExistsInRanges( s_idContinueUnicodeRanges32, code ) )
            {
                return false;
            }
            if ( ExistsInRanges( s_idOtherFormatUnicodeRanges32, code ) )
            {
                return false;
            }
            return ( ( ( c != '@' ) && ( c != '#' ) ) && ( ( ( c != '$' ) && ( c != '_' ) ) && ( code != 0xff3f ) ) );
        }

        protected override bool RequiresQuoting( string identifierPart )
        {
            return RequiresQuoting( identifierPart, ref this._serverMajorVersion, ref this._reservedWords, base.Site );
        }

        internal static bool RequiresQuoting( string identifierPart, ref int _serverMajorVersion, ref string[] _reservedWords, IVsDataConnection connection )
        {
            if ( identifierPart.Length != 0 )
            {
                if ( _serverMajorVersion == 0 )
                {
                    IVsDataSourceInformation service = connection.GetService( typeof( IVsDataSourceInformation ) ) as IVsDataSourceInformation;
                    string str = service[ "DataSourceVersion" ] as string;
                    int.TryParse( str.Split( new char[] { '.' } )[ 0 ], out _serverMajorVersion );
                }
                bool isFirstCharacter = true;
                foreach ( char ch in identifierPart )
                {
                    if ( ( ( _serverMajorVersion <= 8 ) && MustQuoteOnSql8OrBelow( ch, isFirstCharacter ) ) || ( ( _serverMajorVersion > 8 ) && MustQuoteOnSql9OrHigher( ch, isFirstCharacter ) ) )
                    {
                        return true;
                    }
                    isFirstCharacter = false;
                }
                if ( _reservedWords == null )
                {
                    IVsDataSourceInformation information2 = connection.GetService( typeof( IVsDataSourceInformation ) ) as IVsDataSourceInformation;
                    _reservedWords = ( information2[ "ReservedWords" ] as string ).Split( new char[] { ',' } );
                }
                foreach ( string str3 in _reservedWords )
                {
                    if ( identifierPart.Equals( str3, StringComparison.OrdinalIgnoreCase ) )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override string[] SplitIntoParts( string typeName, string identifier )
        {
            if ( typeName == null )
            {
                throw new ArgumentNullException( "typeName" );
            }
            IVsDataObjectSupportModel service = base.Site.GetService( typeof( IVsDataObjectSupportModel ) ) as IVsDataObjectSupportModel;
            if ( !service.Types.ContainsKey( typeName ) )
            {
                throw new ArgumentException( string.Format( null, "Invalid type", new object[] { typeName } ) );
            }
            string[] destinationArray = new string[ service.Types[ typeName ].Identifier.Count ];
            if ( identifier != null )
            {
                int index = 0;
                int startIndex = 0;
                int num3 = 0;
                char ch = '\0';
                while ( num3 < identifier.Length )
                {
                    if ( ( identifier[ num3 ] == '"' ) && ( ch == '\0' ) )
                    {
                        ch = '"';
                    }
                    else if ( ( identifier[ num3 ] == '"' ) && ( ch == '"' ) )
                    {
                        if ( ( num3 < ( identifier.Length - 1 ) ) && ( identifier[ num3 + 1 ] == '"' ) )
                        {
                            num3++;
                        }
                        else
                        {
                            ch = '\0';
                        }
                    }
                    else if ( ( identifier[ num3 ] == '"' ) && ( ch == '\0' ) )
                    {
                        ch = '"';
                    }
                    else if ( ( identifier[ num3 ] == '"' ) && ( ch == '"' ) )
                    {
                        if ( ( num3 < ( identifier.Length - 1 ) ) && ( identifier[ num3 + 1 ] == '"' ) )
                        {
                            num3++;
                        }
                        else
                        {
                            ch = '\0';
                        }
                    }
                    else if ( ( identifier[ num3 ] == '.' ) && ( ch == '\0' ) )
                    {
                        if ( index == destinationArray.Length )
                        {
                            throw new FormatException( "Invalid identifier" );
                        }
                        destinationArray[ index ] = identifier.Substring( startIndex, num3 - startIndex );
                        index++;
                        startIndex = num3 + 1;
                    }
                    num3++;
                }
                if ( identifier.Length > 0 )
                {
                    if ( index == destinationArray.Length )
                    {
                        throw new FormatException( "Invalid identifier" );
                    }
                    destinationArray[ index ] = identifier.Substring( startIndex );
                }
            }
            int destinationIndex = 0;
            for ( int i = destinationArray.Length - 1; i >= 0; i-- )
            {
                if ( destinationArray[ i ] != null )
                {
                    break;
                }
                destinationIndex++;
            }
            string[] sourceArray = destinationArray;
            destinationArray = new string[ sourceArray.Length ];
            Array.Copy( sourceArray, 0, destinationArray, destinationIndex, destinationArray.Length - destinationIndex );
            return destinationArray;
        }

        protected override object UnformatPart( string typeName, string identifierPart )
        {
            if ( identifierPart == null )
            {
                return null;
            }
            string str = identifierPart.Trim();
            if ( str.StartsWith( "\"", StringComparison.Ordinal ) )
            {
                if ( !str.EndsWith( "\"", StringComparison.Ordinal ) )
                {
                    throw new FormatException( "Invalid identifier" );
                }
                return str.Substring( 1, str.Length - 2 ).Replace( "\"\"", "\"" );
            }
            if ( !str.StartsWith( "\"", StringComparison.Ordinal ) )
            {
                return str;
            }
            if ( !str.EndsWith( "\"", StringComparison.Ordinal ) )
            {
                throw new FormatException( "Invalid identifier" );
            }
            return str.Substring( 1, str.Length - 2 ).Replace( "\"\"", "\"" );
        }

        // Nested Types
        private class UnicodeRange
        {
            // Fields
            private int _highCode;
            private int _lowCode;

            // Methods
            public UnicodeRange( int lowCode, int highCode )
            {
                this._lowCode = lowCode;
                this._highCode = highCode;
            }

            // Properties
            public int HighCode
            {
                get
                {
                    return this._highCode;
                }
            }

            public int LowCode
            {
                get
                {
                    return this._lowCode;
                }
            }
        }
    }

}

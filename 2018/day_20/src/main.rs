use std::collections::HashMap;
use std::collections::HashSet;
use std::str::FromStr;
fn main() {
    let reg = "^NNWSSWWNWSWNNEENNES(ENESEESSSEENWNENEESWSESWSESWSEEESESSWSWNN(WSSSWWSSEESESWSESSSSSESWSESSSEESEEEEENEEEESWSWNWSSWWN(E|WSSSENESENEEEESENEEENNNENWWSSSWNWW(WS(WW|EE)|NENWNWNNNENWNWWNNNNWNENNESEENWNNESESEESEENESSSSWWSWWSWNNWSWW(SEESEESSESSWNW(WSESE(EEENWNEEESWSSSSSESSESENEENWNWNNNWN(WSSSE(SSEWNN|)N|NESEENNNNESSSSESEEEENNNWSWNNWNNNEESS(WNSE|)SENNESESWSSEEN(W|NENWNENWNEESESSSENNEENNNESSSSENESESSSWNNWWSSWNW(NNESNWSS|)WSSE(N|SWWSSSEESENNN(WWSEWNEE|)ENEESSW(N|WSEESSE(SWWNW(NEWS|)WSESESSWNWWWNNE(SEWN|)NWNWWWWWWSSSWNWNWWSESWWSEESSWSEESWSESWWNNWSWSWNWSSSSSESWWWWSWSEEEESENN(WWW|EESESWSW(NN|WSWWWWSEEESWWWSWWWS(EEEENEES(EEENWWNEEEENNESSSES(WWNWSNESEE|)ENNWNEEESWSSENENNNWNEENNWNNWSWNWNENWNWWN(WSSWSS(EEE(NW(NEWS|)W|SWSESWS(EES(W|ENNE(NWW(S|N)|E))|W(NNNWESSS|)WSWWS(EE|W)))|WWNENNWW(SEWN|)NEE(ESNW|)N)|EENWNEEENENNWWS(SWNNW(WNN(ESEEN(W|ESENENWNN(ESE(SSSESWW(N|SESENESENEEEE(SWWSEESWWSWNWWN(EENSWW|)WSWNW(N|WSSWW(NEWS|)SEESENN(N|EEEESSSWNNWSSW(NN|SW(NWWNSEES|)SESEESWWSEEESEENEE(SSSSWWWWWS(WWNW(SWEN|)NENN(WSNE|)ESS(S|EEEEN(WWWNSEEE|)E(N|S))|EEEEE)|NNNNWSSWS(E|W(S|NNENW(NEENN(WSWNSENE|)E(NWES|)SS|WS(WWNEWSEE|)SS)))))))|NW(WWWWNE|NNNESS)))|N)|WSSWNN))|N)|SS)|E))|W)|WWWWWNEEEENNEES(ENNNWWS(WWWWNWWNWWSSE(N|EESES(EENWESWW|)WWWN(WSWNNNWSSSSWNNNNNNWWWSESWWWWNWWWSWNWWNNWNWWWNEEEENNNWWWNEENESEEESEESWWWNWSSSENESEEEN(WW|NENEESWSESSWW(N(N|E)|WWSS(WWWNEENWWW(W|SS)|ENESSENNEEENNEENWWNW(SSS|NNENNNWSWS(WWNENENNWSWNNWSWSSWWNNWSWWNWNENNWWNEENNESENENNNNESSSSESENEEEN(ESSE(SWWWW(NEEWWS|)SES(WWWWWWW(S(EEE(SS|E)|S)|NNESEEEN(WWNE|ES))|SEESEENEEEESESSSSESESSSESSSSWW(NNWNE(ESSNNW|)NNWN(E|NWNWNWWNN(W(SSSSENESSE(N|SW(WNW(S|W)|SESE(N(E|NN)|SSWS(EENSWW|)WSWNNWNEE(S|N|E))))|WW)|ESEE(NWNWWEESES|)S))|SEEENES(SWWEEN|)EE(EESEE(NWES|)E|NNW(NENE(S|ENWWWNNWSSW(SSE(SW|NE)|NNW(NNWNNEEEESWSW(NWES|)SEEESSEEENNWNNESE(SSSSSWN(SENNNNSSSSWN|)|NENWWNENEENNE(N(WWSSWNWSSWNWWWNNN(WSW(SSSS(EEESE(ENWNWWWNN(SSEEESNWWWNN|)|SWS(ESSEWNNW|)W)|WNWSW(NNWNWWWWN(EEEEN(ESSENESSW(ENNWSWENESSW|)|W)|WSW(NWSWNSENES|)S)|SS))|N)|ESSEENWNNWNN(SSESSEWNNWNN|))|ENNNW(SS|WNEEESES(W|ENNES(SS|EENWN(ENWNEEENNE(NNN|ESSW(N|SSWSW(NNE|SEEN)))|WWW(WWWNWWS(S|E)|S))))))|SSSE(NEWS|)SWW(SE|NW)))|S)))|S))))|NN)|WNWSWNW(S|NNN(ESSNNW|)WNNWSWNWSWSWNWSSESSENNENE(E|SSWSESWWWSWWNENWN(NNWNNE(NENWNWSWNNEEESESES(ENEES(W|ENNE(SSSS|NWWNE(NWWWNWNWWWSEESWWWNNNENWNENWWSWWNENEENWNENNN(ESSSESS(WNSE|)SSSSEEEE(S(SENESNWSWN|)W|NWNENENE(NWN(EEE|NWNN(ESNW|)WW(NNN|SWNWSSESSEES(SWSSWNNNE(WSSSENSWNNNE|)|ENNWWNEN(SWSEESNWWNEN|))))|S))|WWWNWWWNWNEENNNWSWWNWWSSE(E(EE|SWSWNWWSSSEN(N|EEE(N|ESWSSSSSESWWWSESESWSSWSSWNNWWSESWWWNWWNWNWNNNWWSESSWSSEE(NWES|)SE(N|SWWSWNN(E|WSWSWNNENENWWWS(E|WSSE(N|SWWWNENNWWS(SWNNWWWNEEEENNESEE(SWWEEN|)ENWWNWWNNNWSSWNWWSSE(EESWWWWNWWNENWNNWWSWWNENWWWSWWWNNW(NEESSENNESEEENWWNNNENENNWSWWS(SSSWWWNNESENNNWWNNW(NENNENENEENWNENWNWSSWNWW(SESS(ENEWSW|)W(N|S(SS|E))|NNENENEENNNNWNWWNW(NNNNNEESENNEESWSSENEESENEESWSEEESESSENNNEESWSSESSESWWSEEESESSSSSSWWNENWWWWNEENEN(ESSWENNW|)WWWS(E|WSSSEESWSWSSEEENN(WSWENE|)(NN|EESWSSSWWWWN(EEE|WWNWSWSWS(WNNWWSS(ENSW|)WNWN(WSSES(EE|W)|EN(W|EEN(WW|ENNW(NENWNEESSESENNESSSWWW(N|SS(W|S|ENEEE(S|NN(NNNWWW(S|NENWWWNENENESS(W|EENWNNNNE(SEEESWWWSEESWS(SWSWNSENEN|)EEEE|NWWWSESWWS(EE|WWSE(E|SWW(SSWWS(WSWENE|)EE(E(NN|EE)|S)|NNNENNNN(WSSSWNWNENWNWSW(SESNWN|)NN|EES(SSW(NN|S)|EEE)|N)))))))|E))))|S))))|ESEEEESESEESEENESSENNNENEESSW(SSW(SSWNWSSSSSW(SEES(ENESS(WW|SEE(SWWEEN|)(E|NWNENENWW(NENEES(ESS(WNSE|)ESSSESESEE(NWNWNNNEESS(WNSE|)SENENE(NNEENNWWNWN(EESENNW(ESSWNWESENNW|)|WNWSSSSS(WNWWW(NWNEEEE(SWWEEN|)NWNNWSWWNENENESENNWWNWSWS(SWWW(WSESWWSES(EEN(NNESS(EEE|S)|W)|SWSWNN(WSSSEE|E))|NEENWNENWNWS(WWNNWWNW(NEN(ENNNNESSESWSSENESSW(WWNSEE|)S(S|EENENNW(S|NEENENWNWWNENENNWSWWWNEENNNENNWSWWSWSEE(SWWSWW(WNWNEENNE(SSSWENNN|)ENNENENWNWNWNWSSESS(ENSW|)SWS(E|WNWSW(W|SSENES|NNEEENWWWNNNNWSWNNWWSWSSEE(SENESSWWW(S|WWN(NNNNENENNNWNEESSSSEENWNEENEESSW(WSS(ESSSE(SWEN|)NNEEESENEEENWNWSWNWWS(WN(NNENNNNNENNWWWWNNWNEESSEEEENESSWSSSSW(N|SSSS(ENEEES(WW|SEEENESSWSEEENWNENEENESSSESESWWNWW(NNESNWSS|)SESWWNWSWNWWW(NENWESWS|)SWNWWS(W|SE(SENEENESEEESWWSWSSESWWNNW(N(WSW(WSWENE|)N|NE(S|E))|SSSESS(WNWSNESE|)SEESESSENEENNEESWSEESSSENENWNENNESSESWSSWSESEEEENNNWSWS(E|WNNENNNWNEENESESESENNWNENNNENESENNWNNNWNWNEEENNNNWNENENWNEENWNNNWSSWWWSSSWWSWNNNNWNEENWWWNWNENNWNENN(WWWWSSESE(S(SSWWNWWWWSWWSSWSSENENNESESENESEESENESE(NNWWN(WW(SEWN|)WNWWSE|E)|ESWSESSWWWWSSSSW(SEEEESSWNWSSSWSEESESSWSWWNNE(S|NWWNNWSSWWWNENE(NNW(NEEESS(WN|ENN)|WWSE(E|SWSSWSEEEEEESWSS(WWNENWWSWWSW(NN(WWSSEN|NNN(NESNWS|)W|EE)|SESENNE(WSSWNWESENNE|))|E(SSWNSENN|)(N|EEEENENNESES(W|S|ENNWNENNEE(SWSNEN|)NWNWNWSS(E|SWWW(SEE(SWEN|)E|WNEEENWNNE(EENNENE(NNN(NNE(N(W|E)|SS)|WWSWSE(ENSW|)SWSSWWNWSWWNNNEES(WSNE|)EE(S|N(N|W)))|ESWS(E|W))|S))))))))|S)|E)|NNW(NNNE(SS|ENWWNE(NWWWW(WSWSES(ENESE(NNWWEESS|)SSS|SWNWS(S|WNNENWNWNWNNEE(NE(S|NEENEENNWWWS(EE|SWWNNWWWNWSSWWWNNWWSSE(S(EEESSENNESS(EEN(E|NNW(SS|W))|SWSESWSSWW(SWSEEEEN(WW|ESE(SWWSEWNEEN|)NNWWNE)|NENN(WSNE|)N))|SWNW(S|NNNWSSWNWWS(E|WNWNEEEN(WWWWWWSSENESSESWSSSESESWWSEEEEENWN(NNWNW(NEWS|)S(SESS|WN)|EEE(SWSSWS(EE(S|NN)|WNWSSWNNWWWWWSWNWNWNNESEEES(WW|ENN(E|NWNNNWSSSS(WNWWNNNEE(SWSEWNEN|)NNWSWWNWWNWWN(EEEEES(WW|S|EEN(W|ESSSEESSS(NNNWWNSEESSS|)))|WSSEESWSSWNW(N(E|NNN)|SSSSEENN(WSNE|)ESSSSSSEENE(ESESSWWN(E|WSSWNNWWSESWSSS(ENNEEEEEEN(NNEE(N|SESWSW(SWWSESS(ENNSSW|)WWN(NW(SS|WWNEEE)|E)|NN))|WWW)|WS(E|WNNNE(S|NWNENWNENWNEN(ESSSNNNW|)W)))|N)|NNNWNENWNENWW(NEWS|)S(WNSE|)SSSSS(ENSW|)S)))|E))))|E))|E(S|EEEEEESSENNEEEES(W|ENEEES(WWSSNNEE|)EEESE(NNWWWEEESS|)SES(ES|WWN)))))))|N)))|S(W|S))))|N)|EEESWSEE(WWNENWESWSEE|)))|S)))|E)|NN(W|E))|EEESWSW(N|SSEEESEESWW(SS(EEEN(WW|NENESESSSESESWSWSSWSEEENEEESWSSESWSESWWWNNE(S|NN(N|WWS(WSSSW(NNNNEWSSSS|)SESEN(EEEESSSSENESSSWWWSESENEEENWNEENNNWNWS(SESWENWN|)WNW(S|NEEENWWNEEESSSESSESESSEESSWNWSSESENEENEESENNNENWWSWS(WW(NNNWSWNNWNNNW(WNNEENENNWWS(E|SWNWNWWWNW(S(SS(ENEES(W|E)|SS)|W)|NEENWNWNWWSS(WN(WSWSNENE|)NNNNEESWSEEENENESENNWNEESEENWNNEENEEENNESSEENEESESWW(N|SESENESSEENWNNESEESSEENWNEEESSENNNNNNEESWSEEESSSENNNENWNEN(WWW(SESWENWN|)WWWWWSESSSWWWWWNNWNWN(EESEEN(W|ESSS(EN|WWNE))|WSW(NWSWNWW(S(S|E)|WWWWSESWWNNWWWSESWWNWWWSSWWNNE(N(WWWWSWNWSSESESESESS(WWW(SESNWN|)NN(ESEWNW|)NW(S|NWNWNENWWWSSSE(NN|ES(W(WWNNNNWSSSW(W|N)|S)|E)))|ENESS(S|ENNNE(SS|EEENWN(EEENNSSWWW|)WNWSS(E|WWSWNWNWN(ENWESW|)W))))|EEEE)|S))|SES(E(E|N)|S)))|EESEESESSSWNWWSSWNW(NENN(ESE(N|E)|N)|SSEEEN(EEENNEE(NNWW(SEWN|)NN(ESENSWNW|)W(S|WW)|SWSESSWWSESE(N|SWSSE(N|SSSSWWNWWWWSSESSESESSSWWSSWSEESEENNENNW(SWS(WNSE|)S|NEE(NWWNNNN(ESE(SWSEWNEN|)N|WW(SESNWN|)N(E|W))|SSSSSSW(NN|SSSWWNNE(S|NWWSSWSSWNWWWWNENWNEENWNNEE(S(SESS(ENNSSW|)W(WSEEWWNE|)N|W)|NWWWSWWNENWWNWSSSSWSWWWNNWWNWWSSSWWSWSSSESWSESESWSWSWNWSWNWNWSWNWNNEEES(ESEEENWWNENNWWN(EENNNENNNESSENNNNEEENWNNESEEEENESENEEEESSEEENWNNNWS(SS|WNNNNEEE(SWWSEESSES(W|SESWS(EENSWW|)WWS(EESNWW|)WWN(WNW(W(NEEWWS|)WWSSSSWSWNNWNNW(S(WWW|S)|NEEESWSES(NWNENWESWSES|))|S)|E))|ENWNWNENEENEE(NWWN(WWWWSES(EENWESWW|)SWSSWNNWSWSWNWNEENWNN(ESENN(E(SSS(W|E)|E)|NWSWNNEENWW(EESWWSNEENWW|))|WWW(NN|SSE(NESNWS|)SWWWNNN(ESSNNW|)WW(WWWWNWSSESSWNWSWWWSSENESSESWSSSESESWSESWWNWWSESWSS(SW(NNWWNWNEENWWN(NENEENWWNNNESEN(NWNWNWSWWWNN(NNEESEENNNN(EEN(WW|ESEE(NWES|)SWSSSE(ENWNEN|SWWNWW(NEENWN(WSNE|)E|S(SE(S|N)|WWWSWNN(SSENEEWWSWNN|)))))|W(W|SSS))|WSSW(WN(NESNWS|)W|SSEES(WWW|ENESENN(WWWW|E)|S)))|EESSSW(SESWS(EENSWW|)W(NWSW|SSSW)|N(WW|N)))|WW)|S)|ENEE(SWEN|)EEN(NNENNNNWS(WNWNEENWNNN(WSNE|)ESEESSS(WNNWESSE|)ENEESSSS(ES(W|ENNWNNENWNENWWSWNNE(NWNN(ESNW|)WSSWS(ESSNNW|)WNNNES|EEESE(N|ESWSSW(S(ESWSEEEEENWNWW(SEWN|)NNENESS(W|E(NN(N|EEE)|S))|W)|NN))))|WWW(SSSWENNN|)NNESENN(SSWNWSNESENN|))|SS)|WWW))|NN)))|EEN(W|EE))|SSE(NN|SSWS(WNN(E|W(WNEENSWWSE|)SS)|EE(S|N))))))|W(SS(WWN(NESNWS|)WWS(WNWN(E|WNENNNNW(N|SSSW(NN|SWWSSEE(NWES|)SWWSWNWNNWWWSESSE(ESWWSSWWWNWWWNENESEESENNNWNWNNE(S|NNE(NNWNENNE(SSSEEWWNNN|)NWWSSWWW(SEESS(SSWSWNNWWW(SSSESSE(SWWWSWW(SSSESEESSWNWSWNNWSWWNN(E(E|S)|WWSESWWSS(S|EEN(W|EESSESS(ENNEN(WWNSEE|)EES(ENESSEE(NWNNWNW(S|NNN(WSWNN(ENES|WS)|EESS(WNSE|)ES(SENNEN(WWNSEE|)ESESEEEEESSESWWNWSSEEESSESSWSWSWNWWWNW(SSEEES(SEEESWSSSSWWNENW(NENWESWS|)WSWWS(ES(E(SEEN(EESSW(N|SWNWWW(SEESEESESESSWNWSSWSWNW(NENWNEN(WNWSSS|ES(E|SS))|SSEES(ES(ENE(S|EENNNEN(WNW(NNE(S|NWNNW(SWSESNWNEN|)NENNE(NENWWSWW(SESNWN|)NNE(NNNN(WSWWEENE|)NENEESWSSESEESESES(ENESESWW(SEE(ENNESSENNNNEENNNEESESENNWNENESSESSSSWWN(ENNSSW|)WWNW(N|SSESSEEN(WNSE|)ESENEESSSSWWSSENESSESW(SEE(NNENNEEENESSE(NENNW(S|WWWWNENEES(W|EENWNNWSWNNNWNNNWSWSESSWWWNE(NNNENEENEEEENNWSWNNENENNESEESWSSSSSW(NNNN(N|W)|SWNWWWSESE(N|SW(WNSE|)SSENENEN(W|EEN(W|E(SSSWSWSSEE(SSSWWNN(ESNW|)W(SSSSEEN(ESSSSWS(ESNW|)WNNN(ESNW|)WW|W)|NNW(NENENE|S))|N(N|W))|NNNW(SS|NNE(S|NNNNWW(SEWN|)NNWSSWWNNNNES(SS|ENESENE(SSWSEWNENN|)NWNEN(NNN|WWWSESWWN(NN|WWWWWN(EE|WWNNNNWSSSWWNNN(WWSESWWNWN(E|WNNWW(N|SESWWWSEESSWW(N(WNSE|)E|SEEENNE(NWES|)ESSEESWWW(SEESENEESENESENE(SESESSWNWWWN(EE|WSWWN(E|N|WSSWSWSESWSESENENNW(NENESSSENNNESESSW(SESSWNWNWSWSSWWS(WWNEN(NWSWNWSSSWWNNNNWSSWSESSEEEE(E(E|SWWWSWSESWWNWNN(WWWWNEEEN(ESNW|)WNWNWNWNNNWSWSSE(N|SWWNWSSES(WWNNWNENWNEESENNENNWSWNNEN(WWWNEENE(NWWSWN(WWSSSSS(WNWSWNNW(S(S|W)|NEESENNW(ESSWNWESENNW|))|EEN(WNNNSSSE|)ESSWS(WNSE|)EE)|N(E|N))|S)|ESEEESESESSWNW(NWNWSNESES|)SSESENENNES(EEENEEE(SSWW(WSSS|NE)|NNWSWNWWN(EN(EEN(ESS(WW|E(S|N(ESNW|)N))|NWW(SEWN|)N(EE|W))|W)|WSS(WNW(S|W)|E(SWEN|)E)))|SSWS(S|W)))|ESENN(E(N|ES(E|WSSSS(WWN(NESNWS|)W|EEN(W|ESSE(NN|ESESWWNWWWN(E|W))))))|W)))|E(S|E)))|NN)|EE)|EEENE(NWES|)SEE(SWSWW(SES(ENSW|)WSSSE(NN|ESENEEE(SWWS(E|SSS(EE|WW(SSENSWNN|)NNE(NWES|)S))|N))|N(WSNE|)E)|EENEENNWSWNW(SSWWEENN|)NEENWNW(N(NWES|)EEEE(NWNENNW(S|WN(EEE|W))|SS(EN|SWNNW))|S)))|N)|S)))|NWWNWSWNW(NWWEES|)S)|NN))))|E(N(EN(EESW|WNN)|W)|SS)))))))))))))|E)))|SWS(SENSWN|)WNNWWSE(WNEESSNNWWSE|))|S)|WWN(E|WSWNNWNENNN(ES(ENE(S|N)|SSS)|WWWWSSWSW(NNENNNE(NW|ESW)|SEENENE(ENWWEESW|)SSSW(WWWSSWW(NEWS|)WWSES(W|SEEEESENE(NWNEEEE(SSSES(SEEWWN|)W|NWN(WWS(E|WNWSSWS(WWNEWSEE|)E)|N))|SS))|N))))))|S)|W)|WWW(SSWENN|)N(NWSWNW(N(E|NN)|S)|E))|S)|SS))|SSSSWWWNEENW(ESWWSEWNEENW|))|ESSSW(SEWN|)N))|W)|WW))|N))|W)|N)|W)|WW)|W)|NEENN(EEESWSW(S(EENEWSWW|)W|N)|WW(SEWN|)NENNWN(WNWSSES(SWWN(NWES|)E|E)|EE(SSS|EE))))|W)))|S)|W)|WSW(S(W|E)|NN(NN|E))))|WWNNNE(SS|ENWWNE(E|NWWWSWSEE(SWSESWS(WSESSWNWW(SSE(N|SSW(WSNE|)N)|WWNWNNENWWW(NNESENN(NNESESSESE(NNNNE(NN(ESE(SWEN|)N|WSWW(SESSNNWN|)WWWN(EN(NWS|ES)|W))|S)|SWW(NWNNSSES|)SEE(ENNSSW|)SSWNW(S|W))|W)|SS(WNSE|)E(SWSNEN|)N))|E)|N)))))|NEN(EENSWW|)WNWW(NE|SE))|ENWN(NNWSNESS|)EESSESE(WNWNNWESSESE|))|NEEEE(NWES|)S)|E)|N(EE|WN(NNWESS|)E))|SES(EEE(NEEWWS|)SS|W)))|NN))))|E)|E(E|S))|N))|WW)))))))))|N))))|E(E|N))))|SS)|S)|E))|NNN)|E))))|SWSSEN)|WWN(WSNE|)E)))))|N)))|W))|W(W|S))|E)|W)|N)|EE))|N(W|N))))|SSEE(NWES|)ES(E(NEWS|)SS(WNSE|)ES(EN|WS)|WWWSSSS(NNNNEEWWSSSS|)))|N)))|W)|WSESES(ENSW|)SWSWNWN(E(N|E)|WSS(ESNW|)WNWN(WWNWWSE(WNEESEWNWWSE|)|E)))|SS))|E)|SE(E|SSS))|EE(SWSNEN|)N(EE|WN(E|N))))|SESSWNWSW(ENESENSWNWSW|))|S)|W)|S)))|WW)|NNNNWNENWWSWWWSSWSEENEE(NWWEES|)SS(ENNSSW|)W(N|SWWW(WN(EEE|NWWNENWW(SS|NWWNEEENNN(ESENESSS(WW(NEWS|)S(EESWSNENWW|)W|EEN(WN|ES))|W(SSWENN|)N)))|SS)))|NN)|N)))))|SSSESS(EENNW(NWES|)S|SW(NN|S))))|SSS(SSS|EE))|E)|SSSSENESEESSESEENESE(SSSSWWSWWSSSESSWSSSENNEENNENNNW(NEEENNNW(NNESE(SSSEESWWSSSWW(NNESNWSS|)SSSSENEEENENENEEESENN(ESENEE(N|SSEESSWNWWN(N|WWSESESE(EEEESENNENEESSW(SSSESSSWNNWNWSWWWWNWSSSSEENN(WSNE|)ESESSSESSESWWNNWWWSESWWWWWWSSSEEEESESSESWWWWWSWSESSESEENWNNW(S|NEEEESWWSEEENESESWWSWNWSSWSWNWSSWSWS(EEEN(W|NESEEEENWNN(WSSWNSENNE|)EEN(EESWSSSW(NNWESS|)S(EENNEENENWNNENENWWNWNEENWNWSWWSESWW(SEESE(S(WWN|SSSW)|N|E)|NNNEENENNNW(WWWW(NN|WSS(EEN(ESE(NESNWS|)SWWSW(NWSNES|)SESSWN|W)|WNWWN(WSWNSENE|)EE))|NENWNWNEENN(ESEENENWW(S|WNNENNNW(S(WNSE|)S|NNENNW(WNNWSWWWSE(SWSWNNNWN(WSNE|)EEEN(NE(NWES|)ES(WSNE|)EESEEESW(SSSS(SESWSSS(ENNENEENEENNEEEE(SSSWWNENWWSSSWSEESSSWSSWNWSSEESENESEESEES(EEESEN(NWWWNWNW(NN(E(S|E)|WNWNW(SSWSEE(SENSWN|)N|NNESE(E(EE|S(W|S))|NN(WWW|NNN))))|S)|EEESW(SEE(EEE|NNN(E|N)|S)|W))|SWWSSSWWSWNWSSWWNNNNNESS(ENNESSEENWNNN(WWS(E|WNWN(E|WSWSE(E|SSSWNNWSSWWW(NEENNWSWW(NNE(S|ENNN(WWS(ESWENW|)W|EEN(NENENNN(WSWS(E|S)|E(NEWS|)SSEESWWS(E|WSSW(S(SWNWSSES(NWNNESNWSSES|)|E)|N)))|W)))|SS)|SESESWS(ESSENNESEN(EESSW(WWSEESWWWNWSSW(N(N|W)|SSWSS(WWNEWSEE|)EEEEEENNNWSWNN(WSWSS(E(EE|N)|W)|EEESSSSENEES(EEENEES(W|ENNESSENESENENNWWS(E|WNWN(NWSSWNNWSWWNENWWWNEN(WNN(E(NEEEENWW(EESWWWEEENWW|)|S)|WSWSSE(N|SSW(NWSNES|)SSSEEESENEN(ESEWNW|)WWNW(NE|WSE)))|ESEE(NW|ESW))|EEENESESENESESSS(EE(NNNN(NN|W(W|SSS))|EEENESEENW(ESWWNWESEENW|))|WNNWSW(N|S(E|W))))))|W)))|N)|NWNW(N(EESNWW|)W|S))|W)))))|ES(EE|S))|SS))|NWWWNWSWWNNNENEEENE(NWNNNEE(NNWWWWWN(W(W|SSSWSSENEENE(N(WWSNEE|)EE|SSSWW(NEWS|)WS(EEE|WS(E|W(N(N(E|N)|WSWNW(S|WW))|SSSSE(NNN|SES(ENSW|)WSWNNW(NN|S)))))))|EENWNENE(ENWWWNNN(WSSSW(NN|SE(E|SS))|ESEES(ENSW|)WW)|S))|SWSESEEEESEEN(WNSE|)ESS(E|WW))|SSSW(SEENEWSWWN|)NWWS(WSNE|)E))|W)|W)|WW)|WW)|EE)|S)))|WWNNW(SSSEEWWNNN|)W)))|WWWWW)|W))|WWNENNNWSWWWSEE(SWWS(WWWWNEEENWNWWWWSE(SSWWN(WWS(E|WNWSWNNNNESEEENE(NWNENNWNWSS(SSW(SEWN|)NWWWNNWNNNNENEEEESSS(WNWSWS(SSEENNWS(NESSWWEENNWS|)|WNNENEE(WWSWSSNNENEE|))|ENNNESSSSENNENEEENESSWWSW(N|WSESSSSEENNNENW(WSSSNNNE|)NEENNNNWWWNENNNWWNWWNENNNNEEENWWNNWWSWSSWNWWWSWNNWWSSW(NNNEEENWWNEENWWNW(SSS|NENEESS(WNSE|)E(N(NNWWNEENWWNENNESEE(SWWEEN|)NESENNWWW(WWN(WWSW(NNEN(EESWENWW|)W|SSSE(SSSW(NN|S)|NNE(S|N)))|E)|S)|E)|SSSSSENEE(SWEN|)ENWNEE(NN|SEEESSENESEESWSESESSSWNNWNW(SSSS(WNW(S|WNEENWWNEE(WWSEESNWWNEE|))|SSEEEESSE(NNNEEENNWWNNWSSS(EE|W(NNNNNNW(S|NEEEESENE(SSSSWS(EES(WSSENSWNNE|)EEN(W|E(S|N))|WNNWN(EESNWW|)W(NEWS|)S)|NN(WWN(WN(NWSSWNNW(NEWS|)SSWN(WSS(WNW(S|W)|SENESENEES(EE|W))|N)|E)|E)|ESEESEN(NWWEES|)E(EENNSSWW|)S)))|SWWWN(NN|EE)))|SSSES(ENN(EESWSEEN(SWWNENSWSEEN|)|W)|SWWWSW(SSW(WSSENESS(ENEENE(NWNNWSSW(SEWN|)N|SESWS(S|E))|WW(S|WWWWNNW(NNESNWSS|)SS))|NN)|NNNEE(S(E|W)|NNWNW(SSEWNN|)WWNEEE)))))|NN))))|SEE(NN|ESSEEENEN(ESE(NNNENSWSSS|)SWSW(WSWNWSSESESESS(ENNNEE(NWWW(NEWS|)(W|S)|EESWSWW(NEWS|)SEES(WWSNEE|)E)|WWN(E|NWNWSWSWNW(SSS(SSSSSSSSSSESWSES(W|ENNNNWN(EEE|NN))|ENEENES(NWSWWSNEENES|))|NNNNNEN(ESSSE(SWW(SEWN|)NN|N)|W))))|N)|WNWSSWN(SENNESNWSSWN|))))))|E)|SSWWWW))|E)|EE)|EE)|E)))|N)|N)))|WWWNWSWN(WSS(E|SW(SWWNEWSEEN|)NNN)|NN))|EEEE)|SS)|WS(WN|ESWSS))|NNN(WWWW(N(N|WW)|SE(S|EE))|N)))|N)|E))))))))|N))|E)))|WS(E|W))|S)|E))))|E)))))|E))|E)|W))))|NNNNNNWW(WWSNEE|)NN)))))|SS)|NN)|NENWNEESEESENEN(ES|WWWNE))))|E)|SS)$";
    let reg: Reg = reg.parse().unwrap();
    let mut state = State::new();
    state.step(
        &reg,
        Pos {
            row: SIZE / 2,
            col: SIZE / 2,
        },
    );
    let d = state.furthest();
    println!("{}", d);
    let d = state.rooms_further(1000);
    println!("{}", d);
}

#[derive(Debug, Clone)]
struct Room {
    N: bool,
    S: bool,
    E: bool,
    W: bool,
}

#[derive(Clone)]
enum Space {
    Wall,
    Empty,
    Door,
    Unknown,
}

impl From<&Space> for char {
    fn from(sp: &Space) -> char {
        match sp {
            Space::Wall => '#',
            Space::Empty => ' ',
            Space::Door => ' ',
            Space::Unknown => '?',
        }
    }
}

#[derive(Clone, Debug, PartialEq, Hash, Eq, Copy)]
struct Pos {
    row: usize,
    col: usize,
}

struct State {
    // paths: Vec<Vec<Pos>>,
    set: HashSet<(Pos, Pos)>,
}

const SIZE: usize = 100;

impl ToString for State {
    fn to_string(&self) -> String {
        let mut st = String::new();
        for r in 0..SIZE {
            for c in 0..SIZE {
                let spot = Pos { row: r, col: c };
                let spot_right = Pos { row: r, col: c + 1 };
                if self
                    .set
                    .iter()
                    .map(|(spot1, _spot2)| spot1)
                    .collect::<Vec<&Pos>>()
                    .contains(&&spot)
                {
                    // println!("found spot {:?}", spot);
                    st.push('.');
                } else {
                    // println!("no spot {:?}", spot);
                    st.push('#');
                }
                if self.set.contains(&(spot, spot_right)) {
                    st.push('|');
                } else {
                    st.push('#');
                }
            }
            st.push('\n');
            for c in 0..SIZE {
                let spot = Pos { row: r, col: c };
                let spot_below = Pos { row: r + 1, col: c };
                if self.set.contains(&(spot, spot_below)) {
                    // println!("vert found transition {:?} {:?}", spot, spot_below);
                    st.push('-');
                } else {
                    // println!("no transition {:?} {:?}", spot, spot_below);
                    st.push('#');
                }
                st.push('#');
            }
            st.push('\n');
        }
        st
    }
}

impl State {
    fn new() -> Self {
        Self {
            set: HashSet::new(),
        }
    }

    fn surrounding(&self, pos: &Pos) -> Vec<Pos> {
        let mut above = pos.clone();
        above.row -= 1;
        let mut below = pos.clone();
        below.row += 1;
        let mut left = pos.clone();
        left.col -= 1;
        let mut right = pos.clone();
        right.col += 1;

        vec![above, below, left, right]
            .into_iter()
            .filter(|s| self.set.contains(&(pos.clone(), s.clone())))
            .collect()
    }

    /// get the number of steps to each position
    fn dists(&self, start: &Pos) -> HashMap<Pos, DijkstraNode> {
        let mut dists: HashMap<Pos, DijkstraNode> = HashMap::new();
        dists.insert(
            *start,
            DijkstraNode {
                visited: false,
                dist: 0,
                from: None,
                pos: *start,
            },
        );
        let mut unvisited: Vec<Pos> = vec![];
        unvisited.push(*start);
        let mut visited = 0;
        loop {
            if unvisited.len() == 0 {
                break;
            }
            let current = unvisited.pop().unwrap();
            let surrounding = self.surrounding(&current);
            for next in surrounding.iter() {
                let current_distance = dists[&current].dist + 1;
                let mut value = dists.entry(*next).or_insert(DijkstraNode {
                    visited: false,
                    dist: current_distance,
                    from: Some(current),
                    pos: *next,
                });
                if value.dist > current_distance {
                    value.dist = current_distance;
                    value.from = Some(current);
                }
                if !value.visited && !unvisited.contains(next) {
                    unvisited.push(*next);
                }
            }
            dists.get_mut(&current).unwrap().visited = true;
            visited += 1;
            unvisited.sort_by(|a, b| dists[b].dist.cmp(&dists[a].dist));
        }
        dists
    }

    fn furthest(&self) -> usize {
        let dists = self.dists(&Pos {
            row: SIZE / 2,
            col: SIZE / 2,
        });
        let max_dist = dists
            .iter()
            .max_by(|a, b| {
                let v1 = a.1;
                let v2 = b.1;
                v1.dist.cmp(&v2.dist)
            })
            .unwrap()
            .1
            .dist;
        max_dist
    }

    fn rooms_further(&self, min: usize) -> usize {
        self.dists(&Pos {
            row: SIZE / 2,
            col: SIZE / 2,
        })
        .into_iter()
        .filter(|(_, node)| node.dist >= min)
        .count()
    }

    // number of doors.
    /**
     * Each *path* represents a possible path our program can take.
     * Each 'normal' step will try to move EACH tail of each path by that direction
     * &if this fails, remove that path
     *
     * Ors will create a sub path for each sub expression.
     * For each path
     *  for each sub path that doesn't fail
     *    a new path is created (replacing the path that it went from)
     */
    fn step(&mut self, re: &Reg, from: Pos) -> Pos {
        match re {
            Reg::Or(parts) => {
                for group in parts {
                    let mut prev = from.clone();
                    // println!("option group from: {:?}", option_path);
                    for exp in group {
                        let next = self.step(exp, prev);
                        self.set.insert((next, prev));
                        self.set.insert((prev, next));
                        prev = next;
                    }
                }
                return from;
            }
            _ => {
                let mut new_step = from.clone();
                match re {
                    Reg::N => new_step.row -= 1,
                    Reg::S => new_step.row += 1,
                    Reg::E => new_step.col += 1,
                    Reg::W => new_step.col -= 1,
                    _ => {}
                }
                self.set.insert((from, new_step));
                self.set.insert((new_step, from));
                return new_step;
            }
        }
    }
}

#[derive(Clone, Copy, Debug)]
struct DijkstraNode {
    visited: bool,
    dist: usize,
    pos: Pos,
    from: Option<Pos>,
}

#[derive(Debug, PartialEq)]
enum Reg {
    N,
    E,
    W,
    S,
    Or(Vec<Vec<Reg>>),
}

impl FromStr for Reg {
    type Err = Box<dyn std::error::Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut chars = s.chars();
        let mut stack: Vec<Vec<Reg>> = Vec::new();
        stack.push(Vec::new());
        // build a stack of the 'current' reg group we are building.
        // starting with the first outer group
        while let Some(c) = chars.nth(0) {
            match c {
                '^' | '$' => continue,
                '(' => {
                    // push an or onto the current group
                    // push a new group onto the stack
                    stack.last_mut().unwrap().push(Reg::Or(vec![]));
                    stack.push(Vec::new());
                }
                ')' => {
                    // pop the current group
                    // place it into the last element of the last group. (which should be an Or)
                    let cur_group = stack.pop().unwrap();
                    let or = stack.last_mut().unwrap().last_mut().unwrap();
                    match or {
                        Reg::Or(branches) => branches.push(cur_group),
                        _ => panic!("last element isn't an or group?"),
                    }
                }
                '|' => {
                    // Same as the close group, but also add a new group to keep building
                    let cur_group = stack.pop().unwrap();
                    let or = stack.last_mut().unwrap().last_mut().unwrap();
                    match or {
                        Reg::Or(branches) => branches.push(cur_group),
                        _ => panic!("last element isn't an or group?"),
                    }
                    stack.push(Vec::new())
                }
                'N' => stack.last_mut().unwrap().push(Reg::N),
                'S' => stack.last_mut().unwrap().push(Reg::S),
                'E' => stack.last_mut().unwrap().push(Reg::E),
                'W' => stack.last_mut().unwrap().push(Reg::W),
                _ => panic!("I can't handle: {}", c),
            };
        }
        let main = stack.pop().unwrap();
        Ok(Reg::Or(vec![main]))
    }
}

#[cfg(test)]
mod tests {
    use super::*;
    #[test]
    fn test() {
        let reg = "^WNE$";
        let reg: Reg = reg.parse().unwrap();
        assert_eq!(reg, Reg::Or(vec![vec![Reg::W, Reg::N, Reg::E]]))
    }
    #[test]
    fn test_or() {
        let reg = "^W(NE|SW|)$";
        let reg: Reg = reg.parse().unwrap();
        assert_eq!(
            reg,
            Reg::Or(vec![vec![
                Reg::W,
                Reg::Or(vec![vec![Reg::N, Reg::E], vec![Reg::S, Reg::W], vec![]])
            ],])
        )
    }
    #[test]
    fn test_something() {
        let reg = "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$";
        let reg: Reg = reg.parse().unwrap();
        let mut state = State::new();
        state.step(
            &reg,
            Pos {
                row: SIZE / 2,
                col: SIZE / 2,
            },
        );
        // for p in &state.paths{
        //     // println!("{:?}", p);
        // }
        let d = state.furthest();
        // println!("{}", state.to_string());
        assert_eq!(31, d);
    }
}

using System.Collections.Generic;

namespace SFB.Web.ApplicationCore.Entities
{
    //TODO: refactor after DB schema optimization
    public class EfficiencyMetricDataObject
    {
        #region private

        private int urn;
        private int efficiencydecileingroup;
        private int neighbour49;
        private int efficiencydecileneighbour49;
        private decimal fte;
        private string laname;
        private string name;
        private string phase;
        private decimal senpub;
        private decimal ever6pub;
        private decimal progress8;
        private decimal incomepp;
        private int neighbour1;
        private int neighbour2;
        private int neighbour3;
        private int neighbour4;
        private int neighbour5;
        private int neighbour6;
        private int neighbour7;
        private int neighbour8;
        private int neighbour9;
        private int neighbour10;
        private int neighbour11;
        private int neighbour12;
        private int neighbour13;
        private int neighbour14;
        private int neighbour15;
        private int neighbour16;
        private int neighbour17;
        private int neighbour18;
        private int neighbour19;
        private int neighbour20;
        private int neighbour21;
        private int neighbour22;
        private int neighbour23;
        private int neighbour24;
        private int neighbour25;
        private int neighbour26;
        private int neighbour27;
        private int neighbour28;
        private int neighbour29;
        private int neighbour30;
        private int neighbour31;
        private int neighbour32;
        private int neighbour33;
        private int neighbour34;
        private int neighbour35;
        private int neighbour36;
        private int neighbour37;
        private int neighbour38;
        private int neighbour39;
        private int neighbour40;
        private int neighbour41;
        private int neighbour42;
        private int neighbour43;
        private int neighbour44;
        private int neighbour45;
        private int neighbour46;
        private int neighbour47;
        private int neighbour48;
        private int efficiencydecileneighbour1;
        private int efficiencydecileneighbour2;
        private int efficiencydecileneighbour3;
        private int efficiencydecileneighbour4;
        private int efficiencydecileneighbour5;
        private int efficiencydecileneighbour6;
        private int efficiencydecileneighbour7;
        private int efficiencydecileneighbour8;
        private int efficiencydecileneighbour9;
        private int efficiencydecileneighbour10;
        private int efficiencydecileneighbour11;
        private int efficiencydecileneighbour12;
        private int efficiencydecileneighbour13;
        private int efficiencydecileneighbour14;
        private int efficiencydecileneighbour15;
        private int efficiencydecileneighbour16;
        private int efficiencydecileneighbour17;
        private int efficiencydecileneighbour18;
        private int efficiencydecileneighbour19;
        private int efficiencydecileneighbour20;
        private int efficiencydecileneighbour21;
        private int efficiencydecileneighbour22;
        private int efficiencydecileneighbour23;
        private int efficiencydecileneighbour24;
        private int efficiencydecileneighbour25;
        private int efficiencydecileneighbour26;
        private int efficiencydecileneighbour27;
        private int efficiencydecileneighbour28;
        private int efficiencydecileneighbour29;
        private int efficiencydecileneighbour30;
        private int efficiencydecileneighbour31;
        private int efficiencydecileneighbour32;
        private int efficiencydecileneighbour33;
        private int efficiencydecileneighbour34;
        private int efficiencydecileneighbour35;
        private int efficiencydecileneighbour36;
        private int efficiencydecileneighbour37;
        private int efficiencydecileneighbour38;
        private int efficiencydecileneighbour39;
        private int efficiencydecileneighbour40;
        private int efficiencydecileneighbour41;
        private int efficiencydecileneighbour42;
        private int efficiencydecileneighbour43;
        private int efficiencydecileneighbour44;
        private int efficiencydecileneighbour45;
        private int efficiencydecileneighbour46;
        private int efficiencydecileneighbour47;
        private int efficiencydecileneighbour48;
        private int la;
        private decimal? READPROG_supp;
        private decimal? WRITPROG_supp;
        private decimal? MATPROG_supp;

        #endregion

        public int Neighbour1 { get => neighbour1; set => neighbour1 = value; }
        public int Neighbour2 { get => neighbour2; set => neighbour2 = value; }
        public int Neighbour3 { get => neighbour3; set => neighbour3 = value; }
        public int Neighbour4 { get => neighbour4; set => neighbour4 = value; }
        public int Neighbour5 { get => neighbour5; set => neighbour5 = value; }
        public int Neighbour6 { get => neighbour6; set => neighbour6 = value; }
        public int Neighbour7 { get => neighbour7; set => neighbour7 = value; }
        public int Neighbour8 { get => neighbour8; set => neighbour8 = value; }
        public int Neighbour9 { get => neighbour9; set => neighbour9 = value; }
        public int Neighbour10 { get => neighbour10; set => neighbour10 = value; }
        public int Neighbour11 { get => neighbour11; set => neighbour11 = value; }
        public int Neighbour12 { get => neighbour12; set => neighbour12 = value; }
        public int Neighbour13 { get => neighbour13; set => neighbour13 = value; }
        public int Neighbour14 { get => neighbour14; set => neighbour14 = value; }
        public int Neighbour15 { get => neighbour15; set => neighbour15 = value; }
        public int Neighbour16 { get => neighbour16; set => neighbour16 = value; }
        public int Neighbour17 { get => neighbour17; set => neighbour17 = value; }
        public int Neighbour18 { get => neighbour18; set => neighbour18 = value; }
        public int Neighbour19 { get => neighbour19; set => neighbour19 = value; }
        public int Neighbour20 { get => neighbour20; set => neighbour20 = value; }
        public int Neighbour21 { get => neighbour21; set => neighbour21 = value; }
        public int Neighbour22 { get => neighbour22; set => neighbour22 = value; }
        public int Neighbour23 { get => neighbour23; set => neighbour23 = value; }
        public int Neighbour24 { get => neighbour24; set => neighbour24 = value; }
        public int Neighbour25 { get => neighbour25; set => neighbour25 = value; }
        public int Neighbour26 { get => neighbour26; set => neighbour26 = value; }
        public int Neighbour27 { get => neighbour27; set => neighbour27 = value; }
        public int Neighbour28 { get => neighbour28; set => neighbour28 = value; }
        public int Neighbour29 { get => neighbour29; set => neighbour29 = value; }
        public int Neighbour30 { get => neighbour30; set => neighbour30 = value; }
        public int Neighbour31 { get => neighbour31; set => neighbour31 = value; }
        public int Neighbour32 { get => neighbour32; set => neighbour32 = value; }
        public int Neighbour33 { get => neighbour33; set => neighbour33 = value; }
        public int Neighbour34 { get => neighbour34; set => neighbour34 = value; }
        public int Neighbour35 { get => neighbour35; set => neighbour35 = value; }
        public int Neighbour36 { get => neighbour36; set => neighbour36 = value; }
        public int Neighbour37 { get => neighbour37; set => neighbour37 = value; }
        public int Neighbour38 { get => neighbour38; set => neighbour38 = value; }
        public int Neighbour39 { get => neighbour39; set => neighbour39 = value; }
        public int Neighbour40 { get => neighbour40; set => neighbour40 = value; }
        public int Neighbour41 { get => neighbour41; set => neighbour41 = value; }
        public int Neighbour42 { get => neighbour42; set => neighbour42 = value; }
        public int Neighbour43 { get => neighbour43; set => neighbour43 = value; }
        public int Neighbour44 { get => neighbour44; set => neighbour44 = value; }
        public int Neighbour45 { get => neighbour45; set => neighbour45 = value; }
        public int Neighbour46 { get => neighbour46; set => neighbour46 = value; }
        public int Neighbour47 { get => neighbour47; set => neighbour47 = value; }
        public int Neighbour48 { get => neighbour48; set => neighbour48 = value; }
        public int Neighbour49 { get => neighbour49; set => neighbour49 = value; }
        public int Efficiencydecileneighbour1 { get => efficiencydecileneighbour1; set => efficiencydecileneighbour1 = value; }
        public int Efficiencydecileneighbour2 { get => efficiencydecileneighbour2; set => efficiencydecileneighbour2 = value; }
        public int Efficiencydecileneighbour3 { get => efficiencydecileneighbour3; set => efficiencydecileneighbour3 = value; }
        public int Efficiencydecileneighbour4 { get => efficiencydecileneighbour4; set => efficiencydecileneighbour4 = value; }
        public int Efficiencydecileneighbour5 { get => efficiencydecileneighbour5; set => efficiencydecileneighbour5 = value; }
        public int Efficiencydecileneighbour6 { get => efficiencydecileneighbour6; set => efficiencydecileneighbour6 = value; }
        public int Efficiencydecileneighbour7 { get => efficiencydecileneighbour7; set => efficiencydecileneighbour7 = value; }
        public int Efficiencydecileneighbour8 { get => efficiencydecileneighbour8; set => efficiencydecileneighbour8 = value; }
        public int Efficiencydecileneighbour9 { get => efficiencydecileneighbour9; set => efficiencydecileneighbour9 = value; }
        public int Efficiencydecileneighbour10 { get => efficiencydecileneighbour10; set => efficiencydecileneighbour10 = value; }
        public int Efficiencydecileneighbour11 { get => efficiencydecileneighbour11; set => efficiencydecileneighbour11 = value; }
        public int Efficiencydecileneighbour12 { get => efficiencydecileneighbour12; set => efficiencydecileneighbour12 = value; }
        public int Efficiencydecileneighbour13 { get => efficiencydecileneighbour13; set => efficiencydecileneighbour13 = value; }
        public int Efficiencydecileneighbour14 { get => efficiencydecileneighbour14; set => efficiencydecileneighbour14 = value; }
        public int Efficiencydecileneighbour15 { get => efficiencydecileneighbour15; set => efficiencydecileneighbour15 = value; }
        public int Efficiencydecileneighbour16 { get => efficiencydecileneighbour16; set => efficiencydecileneighbour16 = value; }
        public int Efficiencydecileneighbour17 { get => efficiencydecileneighbour17; set => efficiencydecileneighbour17 = value; }
        public int Efficiencydecileneighbour18 { get => efficiencydecileneighbour18; set => efficiencydecileneighbour18 = value; }
        public int Efficiencydecileneighbour19 { get => efficiencydecileneighbour19; set => efficiencydecileneighbour19 = value; }
        public int Efficiencydecileneighbour20 { get => efficiencydecileneighbour20; set => efficiencydecileneighbour20 = value; }
        public int Efficiencydecileneighbour21 { get => efficiencydecileneighbour21; set => efficiencydecileneighbour21 = value; }
        public int Efficiencydecileneighbour22 { get => efficiencydecileneighbour22; set => efficiencydecileneighbour22 = value; }
        public int Efficiencydecileneighbour23 { get => efficiencydecileneighbour23; set => efficiencydecileneighbour23 = value; }
        public int Efficiencydecileneighbour24 { get => efficiencydecileneighbour24; set => efficiencydecileneighbour24 = value; }
        public int Efficiencydecileneighbour25 { get => efficiencydecileneighbour25; set => efficiencydecileneighbour25 = value; }
        public int Efficiencydecileneighbour26 { get => efficiencydecileneighbour26; set => efficiencydecileneighbour26 = value; }
        public int Efficiencydecileneighbour27 { get => efficiencydecileneighbour27; set => efficiencydecileneighbour27 = value; }
        public int Efficiencydecileneighbour28 { get => efficiencydecileneighbour28; set => efficiencydecileneighbour28 = value; }
        public int Efficiencydecileneighbour29 { get => efficiencydecileneighbour29; set => efficiencydecileneighbour29 = value; }
        public int Efficiencydecileneighbour30 { get => efficiencydecileneighbour30; set => efficiencydecileneighbour30 = value; }
        public int Efficiencydecileneighbour31 { get => efficiencydecileneighbour31; set => efficiencydecileneighbour31 = value; }
        public int Efficiencydecileneighbour32 { get => efficiencydecileneighbour32; set => efficiencydecileneighbour32 = value; }
        public int Efficiencydecileneighbour33 { get => efficiencydecileneighbour33; set => efficiencydecileneighbour33 = value; }
        public int Efficiencydecileneighbour34 { get => efficiencydecileneighbour34; set => efficiencydecileneighbour34 = value; }
        public int Efficiencydecileneighbour35 { get => efficiencydecileneighbour35; set => efficiencydecileneighbour35 = value; }
        public int Efficiencydecileneighbour36 { get => efficiencydecileneighbour36; set => efficiencydecileneighbour36 = value; }
        public int Efficiencydecileneighbour37 { get => efficiencydecileneighbour37; set => efficiencydecileneighbour37 = value; }
        public int Efficiencydecileneighbour38 { get => efficiencydecileneighbour38; set => efficiencydecileneighbour38 = value; }
        public int Efficiencydecileneighbour39 { get => efficiencydecileneighbour39; set => efficiencydecileneighbour39 = value; }
        public int Efficiencydecileneighbour40 { get => efficiencydecileneighbour40; set => efficiencydecileneighbour40 = value; }
        public int Efficiencydecileneighbour41 { get => efficiencydecileneighbour41; set => efficiencydecileneighbour41 = value; }
        public int Efficiencydecileneighbour42 { get => efficiencydecileneighbour42; set => efficiencydecileneighbour42 = value; }
        public int Efficiencydecileneighbour43 { get => efficiencydecileneighbour43; set => efficiencydecileneighbour43 = value; }
        public int Efficiencydecileneighbour44 { get => efficiencydecileneighbour44; set => efficiencydecileneighbour44 = value; }
        public int Efficiencydecileneighbour45 { get => efficiencydecileneighbour45; set => efficiencydecileneighbour45 = value; }
        public int Efficiencydecileneighbour46 { get => efficiencydecileneighbour46; set => efficiencydecileneighbour46 = value; }
        public int Efficiencydecileneighbour47 { get => efficiencydecileneighbour47; set => efficiencydecileneighbour47 = value; }
        public int Efficiencydecileneighbour48 { get => efficiencydecileneighbour48; set => efficiencydecileneighbour48 = value; }
        public int Efficiencydecileneighbour49 { get => efficiencydecileneighbour49; set => efficiencydecileneighbour49 = value; }

        public int Urn { get => urn; set => urn = value; }
        public string Laname { get => laname; set => laname = value; }
        public string Name { get => name; set => name = value; }
        public string Phase { get => phase; set => phase = value; }
        public decimal Senpub { get => senpub; set => senpub = value; }
        public decimal Ever6pub { get => ever6pub; set => ever6pub = value; }
        public decimal Progress8 { get => progress8; set => progress8 = value; }
        public decimal Incomepp { get => incomepp; set => incomepp = value; }
        public int Efficiencydecileingroup { get => efficiencydecileingroup; set => efficiencydecileingroup = value; }
        public int La { get => la; set => la = value; }
        public decimal Fte { get => fte; set => fte = value; }
        public decimal? Writprog_supp { get => WRITPROG_supp; set => WRITPROG_supp = value; }
        public decimal? Readprog_supp { get => READPROG_supp; set => READPROG_supp = value; }
        public decimal? Matprog_supp { get => MATPROG_supp; set => MATPROG_supp = value; }

        public List<EfficiencyMetricNeighbourListItemObject> NeighbourRecords
        {
            get
            {
                return new List<EfficiencyMetricNeighbourListItemObject>
                {
                    new EfficiencyMetricNeighbourListItemObject(Neighbour1, Efficiencydecileneighbour1),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour2, Efficiencydecileneighbour2),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour3, Efficiencydecileneighbour3),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour4, Efficiencydecileneighbour4),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour5, Efficiencydecileneighbour5),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour6, Efficiencydecileneighbour6),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour7, Efficiencydecileneighbour7),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour8, Efficiencydecileneighbour8),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour9, Efficiencydecileneighbour9),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour10, Efficiencydecileneighbour10),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour11, Efficiencydecileneighbour11),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour12, Efficiencydecileneighbour12),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour13, Efficiencydecileneighbour13),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour14, Efficiencydecileneighbour14),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour15, Efficiencydecileneighbour15),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour16, Efficiencydecileneighbour16),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour17, Efficiencydecileneighbour17),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour18, Efficiencydecileneighbour18),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour19, Efficiencydecileneighbour19),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour20, Efficiencydecileneighbour20),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour21, Efficiencydecileneighbour21),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour22, Efficiencydecileneighbour22),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour23, Efficiencydecileneighbour23),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour24, Efficiencydecileneighbour24),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour25, Efficiencydecileneighbour25),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour26, Efficiencydecileneighbour26),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour27, Efficiencydecileneighbour27),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour28, Efficiencydecileneighbour28),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour29, Efficiencydecileneighbour29),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour30, Efficiencydecileneighbour30),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour31, Efficiencydecileneighbour31),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour32, Efficiencydecileneighbour32),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour33, Efficiencydecileneighbour33),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour34, Efficiencydecileneighbour34),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour35, Efficiencydecileneighbour35),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour36, Efficiencydecileneighbour36),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour37, Efficiencydecileneighbour37),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour38, Efficiencydecileneighbour38),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour39, Efficiencydecileneighbour39),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour40, Efficiencydecileneighbour40),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour41, Efficiencydecileneighbour41),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour42, Efficiencydecileneighbour42),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour43, Efficiencydecileneighbour43),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour44, Efficiencydecileneighbour44),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour45, Efficiencydecileneighbour45),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour46, Efficiencydecileneighbour46),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour47, Efficiencydecileneighbour47),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour48, Efficiencydecileneighbour48),
                    new EfficiencyMetricNeighbourListItemObject(Neighbour49, Efficiencydecileneighbour49),
                };
            }
        }
    }

    public class EfficiencyMetricNeighbourListItemObject
    {
        public EfficiencyMetricNeighbourListItemObject(int urn, int rank)
        {
            URN = urn;
            Rank = rank;
        }
        public int URN { get; set; }

        public int Rank { get; set; }
    }
}
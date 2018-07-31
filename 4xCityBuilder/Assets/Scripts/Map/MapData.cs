public class MapData
{
	private short[,] surfaceValue;
	private byte[,]  groundValue;
	private byte[,]  stoneValue;
	private byte[,]  undergroundValue;
	private byte[,]  specialValue;

	public MapData(int N)
	{
		surfaceValue     = new short[N, N];
		groundValue      = new byte[N, N];
		stoneValue       = new byte[N, N];
        undergroundValue = new byte[N, N];
		specialValue     = new byte[N, N];
		
		for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                undergroundValue[i, j] = 0; // Nothing underground except stone = 0
                stoneValue[i, j] = 99; //Everything stone will be replaced
				surfaceValue[i, j] = -1; // Nothing on the surface = -1
            }
        }
		
	}
	
	public void SetGroundValue(int i, int j, byte val)
    { groundValue[i,j] = val; }

    public void SetUndergroundValue(int i, int j, byte val)
    { undergroundValue[i,j] = val; }

    public void SetStoneValue(int i, int j, byte val)
    { stoneValue[i,j] = val; }

    public void SetSpecialValue(int i, int j, byte val)
    { specialValue[i,j] = val; }

    public void SetSurfaceValue(int i, int j, short val)
	{ surfaceValue[i,j] = val; }
	
	public void SetGround(byte[,] gv)
    { groundValue = gv; }

    public void SetUnderground(byte[,] ugv)
    { undergroundValue = ugv; }

    public void SetStone(byte[,] sv)
    { stoneValue = sv; }

    public void SetSpecial(byte[,] sv)
    { specialValue = sv; }

    public void SetSurface(short[,] sv)
	{ surfaceValue = sv; }
	
	public byte GetGroundValue(int i, int j)
    { return groundValue[i, j]; }

    public byte GetUndergroundValue(int i, int j)
    { return undergroundValue[i, j]; }

    public byte GetStoneValue(int i, int j)
    { return stoneValue[i, j]; }

    public byte GetSpecialValue(int i, int j)
    { return specialValue[i, j]; }

    public short GetSurfaceValue(int i, int j)
    { return surfaceValue[i, j]; }
}
using System.Globalization;

namespace Steamworks
{
    public readonly struct CGameID
    { 
        private readonly ulong _id;

        public CGameID(ulong id)
        {
            _id = id;
        }

        public override int GetHashCode() => _id.GetHashCode();
        public override string ToString() => _id.ToString(NumberFormatInfo.InvariantInfo);

        public static implicit operator CGameID(ulong id) => new(id);
        public static implicit operator ulong(CGameID steamId) => steamId._id;
    }
}
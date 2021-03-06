using System.Globalization;

namespace Steamworks
{
    public readonly struct CSteamID
    {
        public AccountID_t AccountId => (uint) (_id & 0xFFFFFFFFUL);
        public uint AccountInstance => (uint) ((_id >> 32) & 0xFFFFFUL);
        public EAccountType AccountType => (EAccountType) ((_id >> 52) & 0xFUL);
        public EUniverse Universe => (EUniverse) ((_id >> 56) & 0xFFUL);

        private readonly ulong _id;

        public CSteamID(ulong id)
        {
            _id = id;
        }

        public bool IsValid()
        {
            if (AccountType is <= EAccountType.Invalid or >= EAccountType.Max)
                return false;

            if (Universe is <= EUniverse.Invalid or >= EUniverse.Max)
                return false;

            if (AccountType == EAccountType.Individual)
            {
                if (AccountId == 0 || AccountInstance != SteamConstants.k_unSteamUserDefaultInstance)
                    return false;
            }

            if (AccountType == EAccountType.Clan)
            {
                if (AccountId == 0 || AccountInstance != 0)
                    return false;
            }

            if (AccountType == EAccountType.GameServer)
            {
                if (AccountId == 0)
                    return false;
                // Any limit on instances?  We use them for local users and bots
            }

            return true;
        }

        public override int GetHashCode() => _id.GetHashCode();
        public override string ToString() => _id.ToString("D64", NumberFormatInfo.InvariantInfo);

        public static implicit operator CSteamID(ulong id) => new(id);
        public static implicit operator ulong(CSteamID steamId) => steamId._id;
    }
}
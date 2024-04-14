using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PFDB.WeaponUtility
{
	public class WeaponIdentification
	{
		private static short _dummyDigit = 1;
		private short _checksum = 0; //temporary
		private long _underlyingIntegerCode;
		private readonly PhantomForcesVersion _version;
		private readonly int _category;
		private readonly int _rank;
		private readonly int _rankTieBreaker;

		public long ID { get { return _underlyingIntegerCode; } }
		public PhantomForcesVersion Version { get { return _version; } }
		public int Category { get { return _category; } }
		public int Rank { get { return _rank; } }
		public int RankTieBreaker { get { return _rankTieBreaker; } }

		public WeaponIdentification(PhantomForcesVersion version, int category, int rank, int rankTieBreaker)
		{
			if (category > 19) return; //invalid
			if(rank > 10001) return; //invalid for now (todo: review ranks)
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_dummyDigit);

			_version = Version;
			_category = category;
			_rank = rank;
			_rankTieBreaker = rankTieBreaker;

			int versionPadding = 4 - _version.VersionNumber.ToString().Length;
			stringBuilder.Append('0', versionPadding);
			stringBuilder.Append(_version.VersionNumber);

			int categoryPadding = 2 - _category.ToString().Length;
			stringBuilder.Append('0', categoryPadding);
			stringBuilder.Append(_category);

			int rankPadding = 7 - _rank.ToString().Length;
			stringBuilder.Append('0', rankPadding);
			stringBuilder.Append(_rank);

			int rankTieBreakerPadding = 2 - _rankTieBreaker.ToString().Length;
			stringBuilder.Append('0', rankTieBreakerPadding);
			stringBuilder.Append(_rankTieBreaker);

			_underlyingIntegerCode = Convert.ToInt64(stringBuilder.ToString());
		}

		public static bool operator ==(WeaponIdentification first, WeaponIdentification second)
		{
			return first.ID == second.ID;
		}
		public static bool operator !=(WeaponIdentification first, WeaponIdentification second)
		{
			return first.ID != second.ID;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (ReferenceEquals(obj, null))
			{
				return false;
			}

			throw new NotImplementedException();
		}
	}
}

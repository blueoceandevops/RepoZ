﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RepoZ.Api.Git;
using RepoZ.UI;
using Tests.Helper;

namespace Tests.UI
{
	public class StatusCompressorTests
	{
		protected RepositoryBuilder _builder;

		[SetUp]
		public void Setup()
		{
			_builder = new RepositoryBuilder();
		}

		protected string Compress(Repository repo) => StatusCompressor.Compress(repo);

		protected string Up => StatusCompressor.SIGN_ARROW_UP;
		protected string Down => StatusCompressor.SIGN_ARROW_DOWN;
		protected string Eq => StatusCompressor.SIGN_IDENTICAL;

		public class CompressMethod : StatusCompressorTests
		{
			[Test]
			public void Returns_Empty_String_For_Null()
			{
				Compress(null).Should().BeEmpty();
			}

			[Test]
			public void Returns_Empty_String_For_Empty_Repositories()
			{
				var repo = _builder.Build();
				Compress(repo).Should().BeEmpty();
			}

			[Test]
			public void Returns_Empty_String_For_Repositories_With_Just_A_Name()
			{
				var repo = _builder
					.WithCurrentBranch("master")
					.Build();

				Compress(repo).Should().BeEmpty();
			}

			[Test]
			public void Returns_Empty_String_For_Repositories_With_Just_A_Name_And_Path()
			{
				var repo = _builder
					.WithCurrentBranch("master")
					.WithName("Repo")
					.Build();

				Compress(repo).Should().BeEmpty();
			}

			[Test]
			public void Returns_An_ArrowUp_And_The_Count_If_AheadBy()
			{
				var repo = _builder
					.WithUpstream()
					.WithAheadBy(15)
					.Build();

				Compress(repo).Should().Be($"{Up}15");
			}

			[Test]
			public void Returns_An_ArrowDown_And_The_Count_If_BehindBy()
			{
				var repo = _builder
					.WithUpstream()
					.WithBehindBy(7)
					.Build();

				Compress(repo).Should().Be($"{Down}7");
			}

			[Test]
			public void Returns_An_ArrowDown_And_An_ArrowDown_And_The_Count_If_AhreadBy_And_BehindBy()
			{
				var repo = _builder
					.WithUpstream()
					.WithAheadBy(15)
					.WithBehindBy(7)
					.Build();

				Compress(repo).Should().Be($"{Up}15 {Down}7");
			}

			[Test]
			public void Returns_An_Empty_String_And_No_Identical_Sign_If_No_Upstream_Is_Set_Even_If_AhreadBy_And_BehindBy()
			{
				var repo = _builder
					.WithoutUpstream()
					.WithAheadBy(15)
					.WithBehindBy(7)
					.Build();

				Compress(repo).Should().BeEmpty();
			}

			[Test]
			public void Returns_An_Identical_Sign_If_AheadBy_And_BehindBy_Is_Zero()
			{
				var repo = _builder
					.WithUpstream()
					.WithAheadBy(0)
					.WithBehindBy(0)
					.Build();

				Compress(repo).Should().Be(Eq);
			}

			[Test]
			public void Returns_An_Identical_Sign_If_AheadBy_And_BehindBy_Is_Not_Set()
			{
				var repo = _builder
					.WithUpstream()
					.Build();

				Compress(repo).Should().Be(Eq);
			}

			[Test]
			public void Returns_An_Empty_String_And_No_Identical_Sign_If_No_Upstream_Is_Set()
			{
				var repo = _builder
					.WithoutUpstream()
					.WithAheadBy(0)
					.WithBehindBy(0)
					.Build();

				Compress(repo).Should().BeEmpty();
			}

			[Test]
			public void Returns_Added_Staged_Removed_Without_Upstream()
			{
				var repo = _builder
					.WithLocalAdded(1)
					.WithLocalStaged(2)
					.WithLocalRemoved(3)
					.Build();

				Compress(repo).Should().Be("+1 ~2 -3");
			}

			[Test]
			public void Returns_Added_Staged_Removed_And_Identical_Sign_With_Upstream()
			{
				var repo = _builder
					.WithUpstream()
					.WithLocalAdded(1)
					.WithLocalStaged(2)
					.WithLocalRemoved(3)
					.Build();

				Compress(repo).Should().Be($"{Eq} +1 ~2 -3");
			}


			[Test]
			public void Returns_Untracked_Modified_Missing_Without_Upstream()
			{
				var repo = _builder
					.WithLocalUntracked(4)
					.WithLocalModified(5)
					.WithLocalMissing(6)
					.Build();

				Compress(repo).Should().Be("+4 ~5 -6");
			}

			[Test]
			public void Returns_Untracked_Modified_Missing_And_Identical_Sign_With_Upstream()
			{
				var repo = _builder
					.WithUpstream()
					.WithLocalUntracked(4)
					.WithLocalModified(5)
					.WithLocalMissing(6)
					.Build();

				Compress(repo).Should().Be($"{Eq} +4 ~5 -6");
			}

			[Test]
			public void Returns_Added_Staged_Removed_Untracked_Modified_Missing_Without_Upstream()
			{
				var repo = _builder
					.WithLocalAdded(1)
					.WithLocalStaged(2)
					.WithLocalRemoved(3)
					.WithLocalUntracked(4)
					.WithLocalModified(5)
					.WithLocalMissing(6)
					.Build();

				Compress(repo).Should().Be("+1 ~2 -3 | +4 ~5 -6");
			}

			[Test]
			public void Returns_Added_Staged_Removed_Untracked_Modified_Missing_And_Identical_Sign_With_Upstream()
			{
				var repo = _builder
					.WithUpstream()
					.WithLocalAdded(1)
					.WithLocalStaged(2)
					.WithLocalRemoved(3)
					.WithLocalUntracked(4)
					.WithLocalModified(5)
					.WithLocalMissing(6)
					.Build();

				Compress(repo).Should().Be($"{Eq} +1 ~2 -3 | +4 ~5 -6");
			}

			[Test]
			public void Returns_Added_Staged_Removed_Untracked_Modified_Missing_Without_AheadBy_And_BehindBy_Without_Upstream()
			{
				var repo = _builder
					.WithoutUpstream()
					.WithAheadBy(15)
					.WithBehindBy(7)
					.WithLocalAdded(1)
					.WithLocalStaged(2)
					.WithLocalRemoved(3)
					.WithLocalUntracked(4)
					.WithLocalModified(5)
					.WithLocalMissing(6)
					.Build();

				Compress(repo).Should().Be("+1 ~2 -3 | +4 ~5 -6");
			}

			[Test]
			public void Returns_Added_Staged_Removed_Untracked_Modified_Missing_And_AheadBy_And_BehindBy_With_Upstream()
			{
				var repo = _builder
					.WithUpstream()
					.WithAheadBy(15)
					.WithBehindBy(7)
					.WithLocalAdded(1)
					.WithLocalStaged(2)
					.WithLocalRemoved(3)
					.WithLocalUntracked(4)
					.WithLocalModified(5)
					.WithLocalMissing(6)
					.Build();

				Compress(repo).Should().Be($"{Up}15 {Down}7 +1 ~2 -3 | +4 ~5 -6");
			}
		}
	}
}
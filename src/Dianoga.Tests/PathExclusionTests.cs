using System;
using System.IO;
using Dianoga.Optimizers;
using Dianoga.Processors;
using Dianoga.Processors.Pipelines.DianogaOptimize;
using FluentAssertions;
using Moq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Xunit;

namespace Dianoga.Tests.Optimizers
{
	public class PathExclusionTests
	{
		[Fact]
		public void BasicTestShouldBeTrue()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("/test");
			var result = processor.IsExcluded("/test");
			result.Should().BeTrue();
		}

		[Fact]
		public void BasicCaseSensitiveTestShouldBeTrue()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("/Test");
			var result = processor.IsExcluded("/test");
			result.Should().BeTrue();
		}

		[Fact]
		public void BasicTestShouldBeFalse()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("/asdf");
			var result = processor.IsExcluded("/test");
			result.Should().BeFalse();
		}

		[Fact]
		public void BasicWildcardStartShouldBeTrue()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("*.jpg");
			var result = processor.IsExcluded("/test/test.jpg");
			result.Should().BeTrue();
		}

		[Fact]
		public void BasicWildcardStartShouldBeFalse()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("*.jpg");
			var result = processor.IsExcluded("/test/test.png");
			result.Should().BeFalse();
		}

		[Fact]
		public void BasicWildcardEndShouldBeTrue()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("/test/*");
			var result = processor.IsExcluded("/test/test.jpg");
			result.Should().BeTrue();
		}

		[Fact]
		public void BasicWildcardEndShouldBeFalse()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("/test/*");
			var result = processor.IsExcluded("/yolo/test.png");
			result.Should().BeFalse();
		}

		[Fact]
		public void BasicWildcardBothShouldBeTrue()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("*/scripts/*");
			var result = processor.IsExcluded("/test/scripts/test.jpg");
			result.Should().BeTrue();
		}

		[Fact]
		public void BasicWildcardBothShouldBeFalse()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("*/scripts/*");
			var result = processor.IsExcluded("/test/styles/test.jpg");
			result.Should().BeFalse();
		}

		[Fact]
		public void BasicWildcardMiddleShouldBeTrue()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("/Project/*.jpg");
			var result = processor.IsExcluded("/Project/test.jpg");
			result.Should().BeTrue();
		}

		[Fact]
		public void BasicWildcardMiddleShouldBeFalse()
		{
			var processor = new PathExclusion();
			processor.AddExclusion("/Project/*.jpg");
			var result = processor.IsExcluded("/Project/test.png");
			result.Should().BeFalse();
		}
	}
}

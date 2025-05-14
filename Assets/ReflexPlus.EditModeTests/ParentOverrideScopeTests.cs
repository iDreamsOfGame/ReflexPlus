﻿using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;

namespace Reflex.EditModeTests
{
    public class ParentOverrideScopeTests
    {
        [Test]
        public void ParentOverrideScope_WillOverrideAnyNewContainerParent_WhileItsNotDisposed()
        {
            var parentOverride = new ContainerBuilder()
                .RegisterValue(95)
                .Build();

            using (new ParentOverrideScope(parentOverride))
            {
                var containerOneWithParentOverride = new ContainerBuilder().Build();
                containerOneWithParentOverride.Parent.Should().Be(parentOverride);
                containerOneWithParentOverride.Single<int>().Should().Be(95);
                
                var containerTwoWithParentOverride = new ContainerBuilder().Build();
                containerTwoWithParentOverride.Parent.Should().Be(parentOverride);
                containerTwoWithParentOverride.Single<int>().Should().Be(95);
            }
            
            var containerWithoutParentOverride = new ContainerBuilder().Build();
            containerWithoutParentOverride.Parent.Should().BeNull();
            containerWithoutParentOverride.HasBinding<int>().Should().BeFalse();
        }
    }
}
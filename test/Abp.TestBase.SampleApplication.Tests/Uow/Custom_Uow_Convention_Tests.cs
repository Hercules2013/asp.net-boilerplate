﻿using System.Linq;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.TestBase.SampleApplication.People;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Uow
{
    public class Custom_Uow_Convention_Tests: SampleApplicationTestBase
    {
        private readonly MyCustomUowClass _customUowClass;

        public Custom_Uow_Convention_Tests()
        {
            _customUowClass = Resolve<MyCustomUowClass>();
        }

        [Fact]
        public void Should_Apply_Custom_UnitOfWork_Convention()
        {
            _customUowClass.GetPeopleCount().ShouldBeGreaterThan(0);
        }
    }

    public class MyCustomUowClass : ITransientDependency
    {
        private readonly IRepository<Person> _personRepository;

        public MyCustomUowClass(IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
        }

        public virtual int GetPeopleCount()
        {
            //GetAll can be only used inside a UOW. This should work since MyCustomUowClass is UOW by custom convention.
            return _personRepository.GetAll().Count();
        }
    }
}

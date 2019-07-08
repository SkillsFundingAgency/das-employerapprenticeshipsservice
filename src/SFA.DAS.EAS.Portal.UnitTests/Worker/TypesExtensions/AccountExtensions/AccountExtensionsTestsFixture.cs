using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.TypesExtensions.AccountExtensions
{
    public class AccountExtensionsTestsFixture<TFixture, TEntity, TKey, TMutateProperty> 
        where TFixture : AccountExtensionsTestsFixture<TFixture, TEntity, TKey, TMutateProperty>
        where TEntity : new()
    {
        public Fixture Fixture { get; set; }
        public Account Account { get; set; }
        public Account OriginalAccount { get; set; }
        public TMutateProperty MutatedEntityProperty { get; set; }
        public Action<TEntity> OnAdd { get; set; }
        public Action<TEntity> OnGet { get; set; }
        public TKey EntityKey { get; set; }
        public readonly Func<Account, ICollection<TEntity>> GetCollection;
        public readonly Func<TEntity, TKey> GetKey;
        public readonly Action<TEntity, TKey> SetKey;
        public readonly Action<TEntity, TMutateProperty> SetMutateProperty;

        public AccountExtensionsTestsFixture(
            Func<Account, ICollection<TEntity>> getCollection,
            //todo: can't ref a property, use expression instead?
            Func<TEntity,TKey> getKey,
            Action<TEntity,TKey> setKey,
            Action<TEntity, TMutateProperty> setMutateProperty)
        {
            GetCollection = getCollection;
            GetKey = getKey;
            SetKey = setKey;
            SetMutateProperty = setMutateProperty;

            Fixture = new Fixture();
            Fixture.Customize<Account>(co => co.Without(x => x.Vacancies));
            Account = Fixture.Create<Account>();
            
            Account.Deleted = null;
            EntityKey = Fixture.Create<TKey>();
        }
        
        public TFixture ArrangeEntityInAccount()
        {
            SetKey(GetCollection(Account).RandomElement(), EntityKey);
            return (TFixture)this;
        }
        
        public TFixture ArrangeOnAddActionGiven()
        {
            OnAdd = OnAction();
            return (TFixture)this;
        }

        public TFixture ArrangeOnGetActionGiven()
        {
            OnGet = OnAction();
            return (TFixture)this;
        }

        public Action<TEntity> OnAction()
        {
            return entity => SetMutateProperty(entity, MutatedEntityProperty = Fixture.Create<TMutateProperty>());
        }
        
        public void AssertEntityCreatedAndAddedToAccount(bool mutated = false)
        {
            GetCollection(Account).Should()
                .BeEquivalentTo(GetCollection(OriginalAccount).Append(ExpectedCreatedEntity(mutated)));
        }

        public void AssertReturnCreatedEntity(TEntity returnedEntity, bool mutated = false)
        {
            returnedEntity.Should()
                .BeEquivalentTo(ExpectedCreatedEntity(mutated));
        }
        
        private TEntity ExpectedCreatedEntity(bool mutated)
        {
            var expectedEntity = new TEntity();
            SetKey(expectedEntity, EntityKey);

            if (mutated)
                SetMutateProperty(expectedEntity, MutatedEntityProperty);

            return expectedEntity;
        }
        
        public void AssertOriginalEntityInAccount()
        {
            GetCollection(Account).Should().BeEquivalentTo(GetCollection(OriginalAccount));
        }
        
        public void AssertExistingEntityMutatedInAccount()
        {
            ExpectedExistingEntity(true);
            GetCollection(Account).Should().BeEquivalentTo(GetCollection(OriginalAccount));
        }
        
        public void AssertReturnExistingEntity(TEntity returnedEntity, bool mutated = false)
        {
            returnedEntity.Should().BeEquivalentTo(ExpectedExistingEntity(mutated));
        }
        
        private TEntity ExpectedExistingEntity(bool mutated = false)
        {
            var expectedExistingEntity = GetCollection(OriginalAccount)
                .Single(o => EqualityComparer<TKey>.Default.Equals(GetKey(o), EntityKey));

            if (mutated)
                SetMutateProperty(expectedExistingEntity, MutatedEntityProperty);
            
            return expectedExistingEntity;
        }
    }
}
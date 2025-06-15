# Fluent Method Chaining Pattern for Page Object Model

This document explains the implementation of a fluent method chaining pattern in our Playwright test automation framework.

## Overview

The fluent method chaining pattern allows for more readable and maintainable test code by enabling method calls to be chained together without breaking the chain. It also avoids the need to create objects of page files each time, improving both readability and efficiency.

## Example Usage

```csharp
PageFactory.GetPage<LoginPage>()
    .EnterUsername("username")
    .EnterPassword("Password")
    .ClickOnLoginButton()
    .HomePage
    .ClickOnSideMenu()
    .ClickOnInventoryMovement()
    .InventoryMovementPage
    .ClickOnNewButton();
```

## Key Components

1. **IFluentPage Interface**: A marker interface that defines the contract for fluent page objects.

2. **PageFactory**: A factory class that creates and caches page instances, allowing them to be reused throughout the test execution.

3. **FluentBasePage**: A base class for all fluent page objects, extending the existing BasePage class and implementing the IFluentPage interface.

4. **Page Properties**: Properties in page classes that provide access to other pages, enabling navigation without creating new instances.

5. **Synchronized Methods**: Wrapper methods that convert async operations to sync for a cleaner fluent API.

## Implementation Details

### Page Factory

The PageFactory class is responsible for creating and caching page instances:

```csharp
public class PageFactory
{
    private readonly ConcurrentDictionary<Type, IFluentPage> _pageCache = new();
    
    public T GetPage<T>() where T : class, IFluentPage
    {
        return (T)_pageCache.GetOrAdd(typeof(T), CreatePage<T>);
    }
}
```

### Fluent Page Classes

Each page class extends FluentBasePage and implements fluent methods:

```csharp
public class LoginPage : FluentBasePage
{
    private HomePage _homePage;
    public HomePage HomePage => _homePage ??= PageFactory.GetPage<HomePage>();
    
    public LoginPage EnterUsername(string username)
    {
        // Implementation
        return this;
    }
    
    public HomePage ClickOnLoginButton()
    {
        // Implementation
        return HomePage;
    }
}
```

### Page Navigation

Navigation between pages is handled through properties that return instances of other pages:

```csharp
public HomePage HomePage => _homePage ??= PageFactory.GetPage<HomePage>();
```

### Method Chaining

Methods that perform actions on the current page return the page instance:

```csharp
public LoginPage EnterUsername(string username)
{
    // Implementation
    return this;
}
```

Methods that navigate to a different page return an instance of the new page:

```csharp
public HomePage ClickOnLoginButton()
{
    // Implementation
    return HomePage;
}
```

## Benefits

1. **Improved Readability**: Test code is more readable and flows naturally.

2. **Reduced Boilerplate**: No need to create new page objects manually.

3. **Better Encapsulation**: Page navigation logic is encapsulated within the page objects.

4. **Efficient Resource Usage**: Page instances are created once and cached for reuse.

5. **Enhanced Maintainability**: Changes to page navigation require modifications in a single place.

## Best Practices

1. **Return the Appropriate Type**: Methods should return the appropriate page type based on the action.

2. **Keep Methods Focused**: Each method should perform a single action.

3. **Use Properties for Navigation**: Access to other pages should be provided through properties.

4. **Provide Both Async and Sync APIs**: For flexibility, provide both async methods (returning Tasks) and sync methods (for fluent chaining).

5. **Document Navigation Paths**: Clearly document the navigation paths between pages.

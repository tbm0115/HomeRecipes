(function () {
    // This code exists to support functionality in LocalRecipeStore.cs. It provides convenient access to
    // the browser's IndexedDB APIs, along with a preconfigured database structure.

    const db = idb.openDB('Recipes', 1, {
        upgrade(db) {
            db.createObjectStore('metadata');
            db.createObjectStore('serverdata', { keyPath: 'name' }).createIndex('dateModified', 'dateModified');
            db.createObjectStore('localedits', { keyPath: 'name' });
        },
    });

    window.localRecipeStore = {
        get: async (storeName, key) => (await db).transaction(storeName).store.get(key),
        getAll: async (storeName) => (await db).transaction(storeName).store.getAll(),
        getAllKeys: async (storeName) => (await db).transaction(storeName).store.getAllKeys(),
        getFirstFromIndex: async (storeName, indexName, direction) => {
            const cursor = await (await db).transaction(storeName).store.index(indexName).openCursor(null, direction);
            return (cursor && cursor.value) || null;
        },
        put: async (storeName, key, value) => {
            if ((storeName === 'serverData' || storeName === 'localEdits') && !value.name) {
                console.error(`Missing 'name' property in object:`, value);
                throw new Error("Object is missing 'name' property.");
            }
            return (await db).transaction(storeName, 'readwrite').store.put(value, key === null ? undefined : key);
        },
        putAllFromJson: async (storeName, json) => {
            const store = (await db).transaction(storeName, 'readwrite').store;
            JSON.parse(json).forEach(item => {
                if ((storeName === 'serverData' || storeName === 'localEdits') && !item.name) {
                    console.error(`Missing 'name' property in object:`, item);
                    throw new Error("Object is missing 'name' property.");
                }
                store.put(item);
            });
        },
        delete: async (storeName, key) => (await db).transaction(storeName, 'readwrite').store.delete(key),
        autocompleteKeys: async (storeName, text, maxResults) => {
            const results = [];
            let cursor = await (await db).transaction(storeName).store.openCursor(IDBKeyRange.bound(text, text + '\uffff'));
            while (cursor && results.length < maxResults) {
                results.push(cursor.key);
                cursor = await cursor.continue();
            }
            return results;
        }
    };
})();

(function () {
    // Migration functions
    const migrations = [
        // Version 1 to Version 2
        (db, transaction) => {
            // Delete 'serverdata' store if it exists
            if (db.objectStoreNames.contains('serverdata')) {
                db.deleteObjectStore('serverdata');
            }

            // Create 'metadata' store if it doesn't exist
            if (!db.objectStoreNames.contains('metadata')) {
                db.createObjectStore('metadata');
            }

            // Create 'localedits' store if it doesn't exist
            if (!db.objectStoreNames.contains('localedits')) {
                db.createObjectStore('localedits', { keyPath: 'name' });
            }
        },
        // Version 2 to Version 3
        (db, transaction) => {
            // Delete 'localedits' store if it exists
            if (db.objectStoreNames.contains('localedits')) {
                db.deleteObjectStore('localedits');
            }

            // Create 'collections' store if it doesn't exist
            if (!db.objectStoreNames.contains('collections')) {
                db.createObjectStore('collections', { keyPath: 'name' });
            }
        }
    ];

    // Function to run migrations based on version numbers
    const runMigrations = (db, oldVersion, newVersion, transaction) => {
        for (let i = oldVersion; i < newVersion; i++) {
            const migration = migrations[i];
            if (migration) {
                migration(db, transaction);
            }
        }
    };

    // Open the database and run migrations as needed
    const dbPromise = idb.openDB('Recipes', 3, {
        upgrade(db, oldVersion, newVersion, transaction) {
            runMigrations(db, oldVersion, newVersion, transaction);
        }
    });

    // Expose API for localRecipeStore
    window.localRecipeStore = {
        get: async (storeName, key) => (await dbPromise).transaction(storeName).store.get(key),
        getAll: async (storeName) => (await dbPromise).transaction(storeName).store.getAll(),
        getAllKeys: async (storeName) => (await dbPromise).transaction(storeName).store.getAllKeys(),
        getFirstFromIndex: async (storeName, indexName, direction) => {
            const cursor = await (await dbPromise).transaction(storeName).store.index(indexName).openCursor(null, direction);
            return (cursor && cursor.value) || null;
        },
        put: async (storeName, key, value) => {
            if ((storeName === 'localedits' || storeName === 'collections') && !value.name) {
                console.error(`Missing 'name' property in object:`, value);
                throw new Error("Object is missing 'name' property.");
            }
            return (await dbPromise).transaction(storeName, 'readwrite').store.put(value, key === null ? undefined : key);
        },
        putAllFromJson: async (storeName, json) => {
            const store = (await dbPromise).transaction(storeName, 'readwrite').store;
            JSON.parse(json).forEach(item => {
                if ((storeName === 'localedits' || storeName === 'collections') && !item.name) {
                    console.error(`Missing 'name' property in object:`, item);
                    throw new Error("Object is missing 'name' property.");
                }
                store.put(item);
            });
        },
        delete: async (storeName, key) => (await dbPromise).transaction(storeName, 'readwrite').store.delete(key),
        autocompleteKeys: async (storeName, text, maxResults) => {
            const results = [];
            let cursor = await (await dbPromise).transaction(storeName).store.openCursor(IDBKeyRange.bound(text, text + '\uffff'));
            while (cursor && results.length < maxResults) {
                results.push(cursor.key);
                cursor = await cursor.continue();
            }
            return results;
        }
    };
})();

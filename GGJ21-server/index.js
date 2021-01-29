const fastify = require('fastify')({ logger: true });
const { default: fastifyCors } = require('fastify-cors');
const { v4: uuidv4, validate: validate } = require('uuid');
const profanityFilter = new (require('bad-words'))();
const superagent = require('superagent');

const playerProperties = { name: { type: 'string' }, score: { type: 'number' }, ip: { type: 'string' }, countryCode: { type: 'string' }, country: { type: 'string' } };
const itemProperties = { item: { type: 'string' }, room: { type: 'string' }, guid: { type: ['string', 'null'] } };
const itemListProperties = { items: { type: 'array', items: { type: 'object', properties: itemProperties } } };
const statusProperties = { status: { type: 'string' } };

const scores = [];
const players = [];
const lostItems = [];
const foundItems = [];

const getIpFromRequest = (req) => {
    if (req.headers['x-forwarded-for']) {
        return req.headers['x-forwarded-for'];
    } else {
        return req.raw.connection.remoteAddress;
    }
}

/** powered by https://www.geoplugin.com */
const geoPluginAPI = 'http://www.geoplugin.net';
const getLocationFromIp = async (ip) => {
    let countryCode = null, countryName = null;
    try {
        const url = `${geoPluginAPI}/json.gp?ip=${ip}`;
        console.log(url);
        const res = await superagent.get(url);
        if (res.body) {
            countryCode = res.body.geoplugin_countryCode || null;
            countryName = res.body.geoplugin_countryName || null;
        }
    } catch (error) {
        console.error(error);
    }
    return { countryCode, countryName };
}

const findPlayerFromReq = async (req, createWithName) => {
    const playerIp = getIpFromRequest(req);
    playerIndex = players.findIndex((x) => x.ip == playerIp);
    if (playerIndex !== -1) {
        return players[playerIndex];
    } else if (createWithName) {
        const playerCountry = await getLocationFromIp(playerIp);
        const player = { ip: playerIp, name: createWithName, score: 0, ...playerCountry };
        players.push(player);
        return player;
    } else {
        return null;
    }
}

const checkNameForProfanity = (name) => {
    return profanityFilter.isProfane(name);
}

/** Lets the client know the server is running (200 if ok) */
const healthCheckSchema = { response: { 200: { type: 'object', properties: statusProperties } } }
fastify.get('/', healthCheckSchema, async (req, reply) => {
    return { status: 'ok' };
});

/** Create the player if it does not already exist (204 no content) */
const createPlayerSchema = { response: { 200: { type: 'object', properties: statusProperties } }, schema: { body: { type: 'object', properties: { name: { type: 'string' } } } } }
fastify.post('/player', createPlayerSchema, async (req, reply) => {
    const name = req.body.name;
    if (checkNameForProfanity(name)) {
        reply.status(400).send({ status: 'Keep it clean' });
        return;
    }

    await findPlayerFromReq(req, req.body.name);
    reply.status(200).send({ status: 'ok' });
});

/** Get the player (200 if ok, 404 if not found) */
const getPlayerSchema = { response: { 200: { type: 'object', properties: { player: playerProperties, status: statusProperties } } } }
fastify.get('/player', getPlayerSchema, async (req, reply) => {
    const player = await findPlayerFromReq(req);
    if (player == null) {
        reply.status(404).send({ player: null, status: 'Please register first' });
    } else {
        reply.status(200).send({ player: player, status: 'ok' });
    }
});

/** Return a list of all lost items (200 if ok, 401 if no player found) */
const getItemsLostSchema = { response: { 200: { type: 'object', properties: { ...itemListProperties, status: statusProperties } } } }
fastify.get('/items/lost', getItemsLostSchema, async (req, reply) => {
    const player = await findPlayerFromReq(req);
    if (!player) {
        reply.status(401).send({ items: [], status: 'Please register first' })
        return;
    }
    return { items: lostItems, status: 'ok' };
});

/** Add to the list of all lost items (UUID conflict items are ignored) (200 if ok, 401 if no player found) */
const postItemsLostSchema = { schema: { body: { type: 'object', properties: itemListProperties } }, response: 204 }
fastify.post('/items/lost', postItemsLostSchema, async (req, reply) => {
    const player = await findPlayerFromReq(req);
    if (!player) {
        reply.status(401).send({ status: 'Please register first' });
        return;
    }

    const bodyItems = req.body.items;
    const addItems = [];

    bodyItems.forEach(element => {
        if (!validate(element.guid)) {
            element.guid = uuidv4();
            addItems.push(element);
        } else {
            const index = lostItems.findIndex((x) => x.guid == element.guid);
            if (index == -1) addItems.push(element);
        }
    });

    lostItems.push(...addItems);
    reply.code(204).send();
});

/** Move items from lost to found */
const postItemsFoundSchema = { schema: { body: { type: 'object', properties: itemListProperties } }, response: 204 }
fastify.post('/items/found', postItemsFoundSchema, async (req, reply) => {
    const player = await findPlayerFromReq(req);
    if (!player) {
        reply.status(401).send({ status: 'Please register first' });
        return;
    }

    const bodyItems = req.body.items;
    let addScore = 0;

    bodyItems.forEach(element => {
        if (element.guid == '') {
            element.guid = uuidv4();
            foundItems.push(element);
            addScore++;
        } else {
            const index = lostItems.findIndex((x) => x.guid == element.guid);
            if (index != -1) {
                foundItems.push(lostItems[index]);
                lostItems.splice(index, 1);
                addScore++;
            }
        }
    });

    player.score += addScore;
    reply.code(204).send();
});

/** Get count metrics for fun */
const metaTotalsSchema = { response: { 200: { body: { type: 'object', properties: { players: 'number', lostItems: 'number', foundItems: 'number' } } } } }
fastify.get('/meta/totals', metaTotalsSchema, async (req, reply) => {
    return {
        players: players.length,
        lostItems: lostItems.length,
        foundItems: foundItems.length,
    }
});

const start = async () => {
    try {
        await fastify.listen(3000);
    } catch (error) {
        fastify.log.error(err);
        process.exit(1);
    }
}

fastify.register(require('fastify-cors'));;
fastify.register(require('fastify-rate-limit'), {
    max: 10,
    timeWindow: '1 minute',
});

// todo: require something in the header that gets sent from unity for basic troll protection
start();
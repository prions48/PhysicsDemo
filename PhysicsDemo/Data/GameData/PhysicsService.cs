namespace PhysicsDemo.Data.GameData
{
    public class PhysicsService
    {
        private readonly PhysicsContext _context;
        public PhysicsService(PhysicsContext context)
        {
            _context = context;
        }
        #region Game functions
        #region Game loaders
        private PhysicsGame? EnrichGame(PhysicsGame? game)
        {
            if (game == null)
                return null;
            game.Players = _context.PhysicsPlayers.Where(e => e.GameID == game.ID).ToList();
            List<PhysicsTurn> turns = _context.PhysicsTurns.Where(e => game.ID == e.GameID).ToList();
            foreach (PhysicsPlayer player in game.Players)
            {
                player.Turns = turns.Where(e => e.PlayerID == player.ID).ToList();
            }
            return game;
        }
        private List<PhysicsGame> EnrichGames(List<PhysicsGame> games)
        {
            List<PhysicsPlayer> players = _context.PhysicsPlayers.Where(e => games.Select(f => f.ID).Contains(e.GameID)).ToList();
            List<PhysicsTurn> turns = _context.PhysicsTurns.Where(e => games.Select(f => f.ID).Contains(e.GameID)).ToList();
            foreach (PhysicsGame game in games)
            {
                game.Players = players.Where(e => e.GameID == game.ID).ToList();
                foreach (PhysicsPlayer player in game.Players)
                {
                    player.Turns = turns.Where(e => e.PlayerID == player.ID).ToList();
                }
            }
            return games;
        }
        public List<PhysicsGame> GetGamesByCreator(Guid creatorid)
        {
            return EnrichGames(_context.PhysicsGames.Where(e => e.CreatorID == creatorid).ToList());
        }
        public List<PhysicsGame> GetOpenGames()
        {
            return EnrichGames(_context.PhysicsGames.Where(e => e.GameEnd == null).ToList());
        }
        public PhysicsGame? GetGameByCode(string code)
        {
            return EnrichGame(_context.PhysicsGames.FirstOrDefault(e => e.JoinCode == code && e.GameEnd == null));
        }
        public PhysicsGame? GetGameByID(Guid id)
        {
            return EnrichGame(_context.PhysicsGames.FirstOrDefault(e => e.ID == id));
        }
        public List<PhysicsPlayer> ReloadPlayers(Guid gameid)
        {
            List<PhysicsPlayer> players = _context.PhysicsPlayers.Where(e => e.GameID == gameid).ToList();
            List<PhysicsTurn> turns = _context.PhysicsTurns.Where(e => gameid == e.GameID).ToList();
            foreach (PhysicsPlayer player in players)
            {
                player.Turns = turns.Where(e => e.PlayerID == player.ID).ToList();
            }
            return players;
        }
        #endregion

        #region Game creators/editors
        public void CreateGame(PhysicsGame game)
        {
            _context.PhysicsGames.Add(game);
            _context.SaveChanges();
        }
        public void CreatePlayer(PhysicsPlayer player)
        {
            _context.PhysicsPlayers.Add(player);
            _context.SaveChanges();
        }
        public void RemovePlayer(PhysicsPlayer player)
        {
            _context.PhysicsPlayers.Remove(player);
            _context.SaveChanges();
        }
        public void CreateTurn(PhysicsTurn turn)
        {
            _context.PhysicsTurns.Add(turn);
            _context.SaveChanges();
        }
        #endregion
        #endregion
    }
}
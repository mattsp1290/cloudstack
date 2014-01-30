namespace VMware.VimAutomation.Types {
   public interface VICredentialStoreItem {
      string Host { get; }
      string User { get; }
      string Password { get; }
      string File { get; }
   }
}